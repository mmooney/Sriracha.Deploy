using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IParameterEvaluator
	{
		string EvaluateBuildParameter(string parameterName, DeployBuild build);
		string EvaluateDeployParameter(string parameterName, RuntimeSystemSettings runtimeSettings, DeployMachine machine, DeployComponent component);
        string ReplaceParameter(string input, string prefix, string fieldName, string fieldValue);
    }
}
