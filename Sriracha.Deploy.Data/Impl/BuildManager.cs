using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class BuildManager: IBuildManager
	{
		private readonly IFileRepository _fileRepository;
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;

		public BuildManager(IBuildRepository buildRepository, IFileRepository fileRepository, IProjectRepository projectRepository)
		{
			this._fileRepository = DIHelper.VerifyParameter(fileRepository);
			this._buildRepository = DIHelper.VerifyParameter(buildRepository);
			this._projectRepository = DIHelper.VerifyParameter(projectRepository);
		}

		public IEnumerable<DeployBuild> GetBuildList(string projectId = null, string branchId = null, string componentId = null)
		{
			return this._buildRepository.GetBuildList(projectId, branchId, componentId);
		}

		public PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions)
		{
			return this._buildRepository.GetBuildList(listOptions);
		}

		public DeployBuild CreateBuild(string projectId, string componentId, string branchId, string fileName, byte[] fileData, string version)
		{
			var project = _projectRepository.TryGetProject(projectId);
			if(project == null)
			{
				project = _projectRepository.GetProjectByName(projectId);
			}
			var branch = _projectRepository.GetBranch(project, branchId);
			var component = _projectRepository.GetComponent(project, componentId);
			var file = this._fileRepository.CreateFile(fileName, fileData);
			return this._buildRepository.CreateBuild(projectId, project.ProjectName, componentId, component.ComponentName, branchId, branch.BranchName, file.Id, version);
		}

		public DeployBuild GetBuild(string buildId)
		{
			return this._buildRepository.GetBuild(buildId);
		}

		public DeployBuild CreateBuild(string projectId, string componentId, string branchId, string fileId, string version)
		{
			var project = _projectRepository.TryGetProject(projectId);
			if (project == null)
			{
				project = _projectRepository.TryGetProjectByName(projectId);
			}
			if(project == null)
			{
				project = _projectRepository.CreateProject(projectId, false);
			}
			var branch = _projectRepository.TryGetBranch(project, branchId);
			if(branch == null)
			{
				branch = _projectRepository.TryGetBranchByName(project, branchId);
			}
			if(branch == null)
			{
				branch = _projectRepository.CreateBranch(project.Id, branchId);
			}
			var component = _projectRepository.TryGetComponent(project, componentId);
			if(component == null)
			{
				component = _projectRepository.TryGetComponentByName(project, componentId);
			}
			if(component == null)
			{
				component = _projectRepository.CreateComponent(project.Id, componentId);
			}
			return this._buildRepository.CreateBuild(project.Id, project.ProjectName, component.Id, component.ComponentName, branch.Id, branch.BranchName, fileId, version);
		}

		public DeployBuild UpdateBuild(string buildId, string projectId, string componentId, string branchId, string fileId, string version)
		{
			var project = _projectRepository.GetProject(projectId);
			var branch = _projectRepository.GetBranch(project, branchId);
			var component = _projectRepository.GetComponent(project, componentId);
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
