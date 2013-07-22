using System;
using System.Collections.Generic;
using System.IO;
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
		private readonly IDeployTaskFactory _taskFactory;
		private readonly IFileManager _fileManager;
		private readonly IZipper _zipper;

		public DeployRunner(IBuildRepository buildRepository, IProjectRepository projectRepository, IDeployTaskStatusManager statusManager, IDeployComponentRunner componentRunner, IDeployTaskFactory taskFactory, IFileManager fileManager, IZipper zipper)
		{
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_statusManager = DIHelper.VerifyParameter(statusManager);
			_componentRunner = DIHelper.VerifyParameter(componentRunner);
			_taskFactory = DIHelper.VerifyParameter(taskFactory);
			_fileManager = DIHelper.VerifyParameter(fileManager);
			_zipper = DIHelper.VerifyParameter(zipper);
		}

		public void Deploy(string deployStateId, string environmentId, string buildId, RuntimeSystemSettings systemSettings)
		{
			var build = _buildRepository.GetBuild(buildId);
			var environment = _projectRepository.GetEnvironment(environmentId);
			var component = _projectRepository.GetComponent(build.ProjectComponentId);
			var environmentComponent = environment.GetEnvironmentComponent(component.Id);

			_statusManager.Info(deployStateId, "Building task definition objects");
			var taskDefinitionList = new List<IDeployTaskDefinition>();
			foreach(var step in component.DeploymentStepList)
			{
				var taskDefinition = _taskFactory.CreateTaskDefinition(step.TaskTypeName, step.TaskOptionsJson);
				taskDefinitionList.Add(taskDefinition);
			}
			
			var fileData = _fileManager.GetFile(build.FileId);
			string compressedFilePath = Path.Combine(systemSettings.GetLocalCompressedPackageDirectory(), fileData.FileName);
			_statusManager.Info(deployStateId, string.Format("Extracting deployment package {0} to {1}", fileData.Id, compressedFilePath));
			_fileManager.ExportFile(fileData.Id, compressedFilePath);
			_statusManager.Info(deployStateId, string.Format("Deployment extracted package {0} to {1}", fileData.Id, compressedFilePath));

			string extractedDirectory = systemSettings.GetLocalExtractedDirectory();
			_zipper.ExtractFile(compressedFilePath, extractedDirectory);

			_componentRunner.Run(deployStateId, _statusManager, taskDefinitionList, environmentComponent, systemSettings);
		}
	}
}
