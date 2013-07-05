using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployRequestManager : IDeployRequestManager
	{
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeploymentValidator _validator;

		public DeployRequestManager(IBuildRepository buildRepository, IProjectRepository projectRepository, IDeploymentValidator _validator)
		{
			_buildRepository = buildRepository;
			_projectRepository = projectRepository;
			_validator = _validator;
		}
		public DeployRequestTemplate InitializeDeployRequest(string buildId, string environmentId)
		{
			var returnValue = new DeployRequestTemplate
			{
				Build = _buildRepository.GetBuild(buildId),
				Environment = _projectRepository.GetEnvironment(environmentId),
			};
			returnValue.Component = _projectRepository.GetComponent(returnValue.Build.ProjectComponentId);
			returnValue.ValidationResult = _validator.ValidateDeployment(returnValue.Component, returnValue.Environment);
			return returnValue;
		}
	}
}
