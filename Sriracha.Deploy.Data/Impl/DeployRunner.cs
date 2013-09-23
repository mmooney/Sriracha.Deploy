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

		public void Deploy(string deployStateId, string environmentId, string buildId, List<string> machineIdList, RuntimeSystemSettings systemSettings)
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
			string compressedFilePath = Path.Combine(systemSettings.GetLocalCompressedPackageDirectory(component.Id), fileData.FileName);
			_statusManager.Info(deployStateId, string.Format("Extracting deployment package {0} to {1}", fileData.Id, compressedFilePath));
			_fileManager.ExportFile(fileData.Id, compressedFilePath);
			_statusManager.Info(deployStateId, string.Format("Deployment extracted package {0} to {1}", fileData.Id, compressedFilePath));

			string extractedDirectory = systemSettings.GetLocalExtractedDirectory(component.Id);
			_statusManager.Info(deployStateId, string.Format("Decompressing deployment package {0} to directory {1}", compressedFilePath, extractedDirectory));
			_zipper.ExtractFile(compressedFilePath, extractedDirectory);
			_statusManager.Info(deployStateId, string.Format("Done decompressing deployment package {0} to directory {1}", compressedFilePath, extractedDirectory));

			foreach(var machineId in machineIdList)
			{
				var machine = environment.GetMachine(machineId);
				//string machineDirectory = systemSettings.GetLocalMachineDirectory(machine.MachineName);
				//if(!Directory.Exists(machineDirectory))
				//{
				//	Directory.CreateDirectory(machineDirectory);
				//}
				string machineComponentDirectory = systemSettings.GetLocalMachineComponentDirectory(machine.MachineName, environmentComponent.ComponentId);
				CopyAllFiles(extractedDirectory, machineComponentDirectory);
				_statusManager.Info(deployStateId, string.Format("Copying deployment files from {0} to machine/component directory {1}", extractedDirectory, machineComponentDirectory));

				_statusManager.Info(deployStateId, string.Format("Done copying deployment files from {0} to machine/component  directory {1}", extractedDirectory, machineComponentDirectory));

				_componentRunner.Run(deployStateId, _statusManager, taskDefinitionList, environmentComponent, machine, systemSettings);
			}
		}

		private void CopyAllFiles(string sourcePath, string targetPath)
		{
			DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
			DirectoryInfo targetDirectory = new DirectoryInfo(targetPath);
			CopyAllFiles(sourceDirectory, targetDirectory);
		}

		private void CopyAllFiles(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
		{
			// Check if the target directory exists, if not, create it.
			if (!targetDirectory.Exists)
			{
				targetDirectory.Create();
			}
			// Copy each file into it’s new directory.
			if (targetDirectory.Attributes > 0 && (targetDirectory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				targetDirectory.Attributes &= ~FileAttributes.ReadOnly;
			}
			foreach (FileInfo sourceFile in sourceDirectory.GetFiles())
			{
				string targetFilePath = Path.Combine(targetDirectory.FullName, sourceFile.Name);
				if (File.Exists(targetFilePath))
				{
					var targetFileInfo = new FileInfo(targetFilePath);
					if (targetFileInfo.Attributes > 0 && (targetFileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					{
						targetFileInfo.Attributes &= ~FileAttributes.ReadOnly;
					}
				}
				sourceFile.CopyTo(targetFilePath, true);
			}

			// Copy each subdirectory using recursion.
			foreach (DirectoryInfo sourceSubDirectory in sourceDirectory.GetDirectories())
			{
				if (!sourceSubDirectory.FullName.StartsWith(targetDirectory.FullName))
				{
					DirectoryInfo targetSubDirectory = targetDirectory.CreateSubdirectory(sourceSubDirectory.Name);
					CopyAllFiles(sourceSubDirectory, targetSubDirectory);
				}
			}
		}

	}
}
