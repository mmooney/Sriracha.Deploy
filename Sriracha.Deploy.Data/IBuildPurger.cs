using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IBuildPurger
	{
		void PurgeBuild(DeployBuild build);
		void PurgeBuildIfNecessary(DeployBuild build);
	}
}
