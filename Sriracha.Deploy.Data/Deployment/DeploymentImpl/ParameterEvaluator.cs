﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class ParameterEvaluator : IParameterEvaluator
	{
        private readonly IParameterParser _parameterParser;

        public ParameterEvaluator(IParameterParser parameterParser)
        {
            _parameterParser = DIHelper.VerifyParameter(parameterParser);
        }
		public string EvaluateBuildParameter(string parameterName, DeployBuild build)
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


		public string EvaluateDeployParameter(string parameterName, RuntimeSystemSettings runtimeSettings, DeployMachine machine, DeployComponent component)
		{
			if (string.IsNullOrEmpty(parameterName))
			{
				throw new ArgumentNullException("Missing parameterName");
			}
			if (runtimeSettings == null)
			{
				throw new ArgumentNullException("Missing runtimeSettings");
			}
			switch (parameterName.ToLower())
			{
				case "directory":
					return runtimeSettings.GetLocalMachineComponentDirectory(machine.MachineName, component.Id);
				default:
					throw new ArgumentException(string.Format("Unrecognized deploy parameter \"{0}\"", parameterName));
			}
		}


        public string ReplaceParameter(string input, string prefix, string fieldName, string fieldValue)
        {
            return _parameterParser.ReplaceParameter(input, prefix, fieldName, fieldValue);
        }
    }
}
