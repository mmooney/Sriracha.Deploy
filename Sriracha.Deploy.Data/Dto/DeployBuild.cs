using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployBuild
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectBranchId { get; set; }
		public string ProjectBranchName { get; set; }
		public string ProjectComponentId { get; set; }
		public string ProjectComponentName { get; set; }
		public string FileId { get; set; }
		public string Version { get; set; }

		public string DisplayValue
		{
			get 
			{
				return string.Format("{0} - {1} - {2} - {3}", this.ProjectName, this.ProjectBranchName, this.ProjectComponentName, this.Version);
			}
		}
	}
}
