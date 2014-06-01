using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sriracha.Deploy.Data.Utility.UtilityImpl
{
	public class ParameterParser  : IParameterParser
	{
		/*
		 * \$\{((((MACHINE:)|(ENV:)).+?))\}
		 * \$\{(((MACHINE:).+?))\}
		*/
		public List<string> FindMachineParameters(string value)
		{
			return FindParameters(value, "MACHINE:");
		}

		public List<string> FindEnvironmentParameters(string value)
		{
			return FindParameters(value, "ENV:");
		}

		public List<string> FindBuildParameters(string value)
		{
			return FindParameters(value, "BUILD:");
		}

		public List<string> FindDeployParameters(string value)
		{
			return FindParameters(value, "DEPLOY:");
		}

		private List<string> FindParameters(string value, string prefix)
		{
			var returnList = new List<string>();
			Regex re = new Regex(@"\$\{(((" + prefix + @").+?))\}", RegexOptions.IgnoreCase);
			var matches = re.Matches(value);
			foreach (Match match in matches)
			{
				foreach (Capture c in match.Captures)
				{
					//rip off the ${ and }
					string foundValue = c.Value.Substring(2, c.Value.Length - 3);
					if (foundValue.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
					{
						foundValue = foundValue.Substring(prefix.Length);
					}
					if (!returnList.Contains(foundValue, StringComparer.CurrentCultureIgnoreCase))
					{
						returnList.Add(foundValue);
					}
				}
			}
			return returnList;
		}

        private List<TaskParameter> FindNestedParameters(object options, Func<string, List<string>> findFunction)
        {
            var list = new List<TaskParameter>();
            if (options != null)
            {
                foreach (var propInfo in options.GetType().GetProperties())
                {
                    if (propInfo.PropertyType == typeof(string))
                    {
                        string value = (string)propInfo.GetValue(options, null);
                        if (!string.IsNullOrEmpty(value))
                        {
                            var valueList = findFunction(value);
                            var x = new List<TaskParameter>();
                            foreach(var valueItem in valueList)
                            {
                                var taskParameter = new TaskParameter()
                                {
                                    FieldName = valueItem,
                                    FieldType = EnumTaskParameterType.String 
                                };
                                if(taskParameter.FieldName.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    taskParameter.FieldName = taskParameter.FieldName.Substring("SENSITIVE:".Length);
                                    taskParameter.Sensitive = true;
                                }
                                if(taskParameter.FieldName.StartsWith("OPTIONAL:", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    taskParameter.FieldName = taskParameter.FieldName.Substring("OPTIONAL:".Length);
                                    taskParameter.Optional = true;
                                }
                                if (taskParameter.FieldName.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    taskParameter.FieldName = taskParameter.FieldName.Substring("SENSITIVE:".Length);
                                    taskParameter.Sensitive = true;
                                }
                                x.Add(taskParameter);
                            }
                            if (x != null && x.Any())
                            {
                                list.AddRange(x);
                            }
                        }
                    }
                    else if (propInfo.PropertyType.IsClass)
                    {
                        var value = propInfo.GetValue(options, null);
                        if (value != null)
                        {
                            if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
                            {
                                foreach (var item in (IEnumerable)value)
                                {
                                    var x = this.FindNestedParameters(item, findFunction);
                                    if (x != null && x.Any())
                                    {
                                        list.AddRange(x);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var x = this.FindNestedDeployParameters(value);
                            if (x != null && x.Any())
                            {
                                list.AddRange(x);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public List<TaskParameter> FindNestedDeployParameters(object options)
        {
            return this.FindNestedParameters(options, (i)=>this.FindDeployParameters(i));
        }


        public List<TaskParameter> FindNestedBuildParameters(object options)
        {
            return this.FindNestedParameters(options, (i)=>this.FindBuildParameters(i));
        }

        public List<TaskParameter> FindNestedMachineParameters(object options)
        {
            return this.FindNestedParameters(options, (i)=>this.FindMachineParameters(i));
        }

        public List<TaskParameter> FindNestedEnvironmentParameters(object options)
        {
            return this.FindNestedParameters(options, (i)=>this.FindEnvironmentParameters(i));
        }

        public string ReplaceParameter(string input, string prefix, string fieldName, string fieldValue)
        {
            List<string> replaceStrings = new List<string>();
            replaceStrings.Add(string.Format("${{{0}:{1}}}", prefix, fieldName));
            replaceStrings.Add(string.Format("${{{0}:sensitive:{1}}}", prefix, fieldName));
            replaceStrings.Add(string.Format("${{{0}:optional:{1}}}", prefix, fieldName));
            replaceStrings.Add(string.Format("${{{0}:sensitive:optional:{1}}}", prefix, fieldName));
            replaceStrings.Add(string.Format("${{{0}:optional:sensitive:{1}}}", prefix, fieldName));
            
            string returnValue = input;
            foreach(var replaceString in replaceStrings)
            {
                returnValue = ReplaceString(returnValue, replaceString, fieldValue, StringComparison.CurrentCultureIgnoreCase);
            }
            return returnValue;
        }

        //http://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive
        //http://stackoverflow.com/a/244933/203479
        private static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}
