using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
	public class DeployRequestTemplate
	{
		public DeployBuild Build { get; set; }

		public DeployEnvironment Environment { get; set; }

		public DeploymentValidationResult ValidationResult { get; set; }

		public DeployComponent Component { get; set; }
	}
}
