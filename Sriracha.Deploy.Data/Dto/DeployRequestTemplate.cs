using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployRequestTemplate
	{
		public DeployBuild Build { get; set; }

		public DeployEnvironment Environment { get; set; }

		public DeploymentValidationResult ValidationResult { get; set; }

		public DeployComponent Component { get; set; }
	}
}
