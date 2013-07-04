using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployRunner : IDeployRunner
	{
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeployTaskStatusManager _statusManager;
		private readonly IDeployComponentRunner _componentRunner;
		private readonly IDeployTaskFactory _deployTaskFactory;

		public DeployRunner(IBuildRepository buildRepository, IProjectRepository projectRepository, IDeployTaskStatusManager statusManager, IDeployComponentRunner componentRunner)
		{
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_statusManager = DIHelper.VerifyParameter(statusManager);
			_componentRunner = DIHelper.VerifyParameter(componentRunner);
		}

		public void Deploy(string environmentId, string buildId, RuntimeSystemSettings systemSettings)
		{
			var build = _buildRepository.GetBuild(buildId);
			var environment = _projectRepository.GetEnvironment(environmentId);
			var component = _projectRepository.GetComponent(build.ProjectComponentId);
			var environmentComponent = environment.ComponentList.Single(i=>i.ComponentId == component.Id);

			var taskDefinitionList = new List<IDeployTaskDefinition>();
			foreach(var step in component.DeploymentStepList)
			{
				var taskDefinition = _deployTaskFactory.CreateTaskDefinition(step.TaskTypeName, step.TaskOptionsJson);
				taskDefinitionList.Add(taskDefinition);
			}

			_componentRunner.Run(_statusManager, taskDefinitionList, environmentComponent, systemSettings);
		}
	}
}
