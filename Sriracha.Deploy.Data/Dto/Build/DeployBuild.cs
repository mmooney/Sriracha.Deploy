using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Build
{
	public class DeployBuild : BaseDto
	{
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
                return GetDisplayValue(this);
			}
		}

        public static string GetDisplayValue(DeployBuild build)
        {
            return GetDisplayValue(build.ProjectName, build.ProjectBranchName, build.ProjectComponentName, build.Version);
        }

        public static string GetDisplayValue(string projectName, string branchName, string componentName, string version)
        {
            return string.Format("{0} - {1} - {2} - {3}", projectName, branchName, componentName, version);
        }


		public string SortableVersion
		{
			get
			{
                return GetSortableVersion(this.Version);
			}
		}

		public string SortableDisplayValue
		{
			get
			{
				return string.Format("{0} - {1} - {2} - {3}", this.ProjectName, this.ProjectBranchName, this.ProjectComponentName, this.SortableVersion);
			}
		}

        public static string GetSortableVersion(string input)
        {
            if (string.IsNullOrEmpty(input) || !input.Contains("."))
            {
                return input;
            }
            else
            {
                //left pad every number with up to 10 chars with a 0, this is just for sorting purposes
                return string.Join(".", input.Split('.').Select(i => i.PadLeft(10, '0')));
            }
        }
    }
}
