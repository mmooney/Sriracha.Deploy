using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Build.BuildImpl
{
	public class BuildManager: IBuildManager
	{
		private readonly IFileRepository _fileRepository;
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IProjectNotifier _projectNotifier;

		public BuildManager(IBuildRepository buildRepository, IFileRepository fileRepository, IProjectRepository projectRepository, IProjectNotifier projectNotifier)
		{
			this._fileRepository = DIHelper.VerifyParameter(fileRepository);
			this._buildRepository = DIHelper.VerifyParameter(buildRepository);
			this._projectRepository = DIHelper.VerifyParameter(projectRepository);
			this._projectNotifier = DIHelper.VerifyParameter(projectNotifier);
		}

		public PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions, string projectId = null, string branchId = null, string componentId = null)
		{
			return this._buildRepository.GetBuildList(listOptions, projectId, branchId, componentId);
		}

		public DeployBuild CreateBuild(string projectId, string componentId, string branchId, string fileName, byte[] fileData, string version)
		{
			var project = _projectRepository.TryGetProject(projectId);
			if(project == null)
			{
				project = _projectRepository.GetProjectByName(projectId);
			}
			var branch = _projectRepository.GetBranch(project, branchId);
			var component = _projectRepository.GetComponent(componentId, project.Id);
			var file = this._fileRepository.CreateFile(fileName, fileData);
			var build = this._buildRepository.CreateBuild(projectId, project.ProjectName, componentId, component.ComponentName, branchId, branch.BranchName, file.Id, version);
			_projectNotifier.SendBuildPublishedNotification(project, build);
			return build;
		}

		public DeployBuild GetBuild(string buildId)
		{
			return this._buildRepository.GetBuild(buildId);
		}

		public DeployBuild CreateBuild(string projectId, string componentId, string branchId, string fileId, string version)
		{
			var project = _projectRepository.GetOrCreateProject(projectId);
			var branch = _projectRepository.GetOrCreateBranch(project.Id, branchId, branchId);
			var component = _projectRepository.GetOrCreateComponent(project.Id, componentId, componentId);
			return this._buildRepository.CreateBuild(project.Id, project.ProjectName, component.Id, component.ComponentName, branch.Id, branch.BranchName, fileId, version);
		}

		public DeployBuild UpdateBuild(string buildId, string projectId, string componentId, string branchId, string fileId, string version)
		{
			var project = _projectRepository.GetProject(projectId);
			var branch = _projectRepository.GetBranch(project, branchId);
            var component = _projectRepository.GetComponent(componentId, project.Id);
			return this._buildRepository.UpdateBuild(buildId, projectId, project.ProjectName, componentId, component.ComponentName, branchId, branch.BranchName, fileId, version);
		}

		public void DeleteBuild(string buildId)
		{
			var build = this._buildRepository.GetBuild(buildId);
			this._fileRepository.DeleteFile(build.FileId);
			this._buildRepository.DeleteBuild(buildId);
		}
	}
}
