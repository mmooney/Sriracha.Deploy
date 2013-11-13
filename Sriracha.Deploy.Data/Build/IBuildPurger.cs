using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Build
{
	public interface IBuildPurger
	{
		void PurgeBuild(DeployBuild build);
		void PurgeBuildIfNecessary(DeployBuild build);
	}
}
