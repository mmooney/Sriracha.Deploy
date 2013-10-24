using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Build
{
	public interface IBuildParameterEvaluator
	{
		string Evaluate(string parameterName, DeployBuild build);
	}
}
