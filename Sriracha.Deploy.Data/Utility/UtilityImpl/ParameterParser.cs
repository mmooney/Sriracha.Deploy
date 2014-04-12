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
                            var x = (from i in findFunction(value)
                                     select new TaskParameter
                                     {
                                        FieldName = (i.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase))
                                                        ? i.Substring("SENSITIVE:".Length)
                                                        : i,
                                        FieldType = EnumTaskParameterType.String,
                                        Sensitive = i.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase)
                                     }).ToList();
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
    }
}
