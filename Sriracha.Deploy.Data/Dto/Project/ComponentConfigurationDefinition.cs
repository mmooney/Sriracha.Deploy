using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Dto.Project
{
	public class ComponentConfigurationDefinition
	{
		public string ProjectId { get; set; }
		public string ParentId { get; set; }
		public EnumDeployStepParentType ParentType { get; set; }
		public List<TaskParameter> EnvironmentTaskParameterList { get; set; }
		public List<TaskParameter> MachineTaskParameterList { get; set; }

		public ComponentConfigurationDefinition()
		{
			this.EnvironmentTaskParameterList = new List<TaskParameter>();
			this.MachineTaskParameterList = new List<TaskParameter>();
		}
	}
}
