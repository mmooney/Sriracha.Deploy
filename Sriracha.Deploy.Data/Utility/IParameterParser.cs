using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Utility
{
	//\$\{((((MACHINE:)|(ENV:)).+?))\}
	public interface IParameterParser
	{
		List<string> FindMachineParameters(string value);
		List<string> FindEnvironmentParameters(string value);
		List<string> FindBuildParameters(string value);
		List<string> FindDeployParameters(string value);

        List<TaskParameter> FindNestedDeployParameters(object options);
        List<TaskParameter> FindNestedBuildParameters(object options);
        List<TaskParameter> FindNestedMachineParameters(object options);
        List<TaskParameter> FindNestedEnvironmentParameters(object options);
    }
}
