using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployProject
	{
		public string Id { get; set; }
		public string ProjectName { get; set; }
		public List<DeployProjectBranch> BranchList { get; set; }
		public List<DeployComponent> ComponentList { get; set; }

		public DeployProject()
		{
			this.BranchList = new List<DeployProjectBranch>();
			this.ComponentList = new List<DeployComponent>();
		}
	}
}
