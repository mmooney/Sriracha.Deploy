using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Impl
{
	public class BuildParameterEvaluator : IBuildParameterEvaluator
	{
		public string Evaluate(string parameterName, DeployBuild build)
		{
			if(string.IsNullOrEmpty(parameterName))
			{
				throw new ArgumentNullException("Missing parameterName");
			}
			if(build == null)
			{
				throw new ArgumentNullException("Missing build");
			}
			switch(parameterName.ToLower())
			{
				case "version":
					return build.Version;
				case "majorversion":
					return GetVersionPosition(parameterName, build.Version, 0);
				case "minorversion":
					return GetVersionPosition(parameterName, build.Version, 1);
				case "buildversion":
					return GetVersionPosition(parameterName, build.Version, 2);
				case "revisionversion":
					return GetVersionPosition(parameterName, build.Version, 3);
			}
			throw new ArgumentException(string.Format("Unrecognized build parameter \"{0}\"", parameterName));
		}

		private string GetVersionPosition(string parameterName, string version, int position)
		{
			if(string.IsNullOrEmpty(version))
			{
				throw new ArgumentNullException(string.Format("Unable to retrieve {0}, Version is missing", parameterName));
			}
			var parts = version.Split('.');
			if(position >= parts.Length)
			{
				throw new ArgumentException(string.Format("Unable to parse parameter {0} from version {1}", parameterName, version));
			}
			return parts[position];
		}
	}
}
