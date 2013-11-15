using System;
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
	}
}
