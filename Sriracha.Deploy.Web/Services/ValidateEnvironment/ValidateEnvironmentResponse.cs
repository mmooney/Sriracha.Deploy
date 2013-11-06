using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Web.Services.ValidateEnvironment
{
	public class ValidateEnvironmentResponse
	{
		public DeployBuild Build { get; set; }
		public DeployEnvironment Environment { get; set; }
		public DeployComponent Component { get; set; }
		public DeploymentValidationResult ValidationResult { get; set; }
	}
}