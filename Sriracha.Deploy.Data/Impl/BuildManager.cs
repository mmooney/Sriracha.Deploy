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

		public DeployBuild SubmitBuild(string projectId, string branchId, string fileName, byte[] fileData, Version version)
		{
			var project = _projectRepository.GetProject(projectId);
			var branch = _projectRepository.GetBranch(project, branchId);
			var file = this._fileRepository.StoreFile(fileName, fileData);
			return this._buildRepository.StoreBuild(projectId, project.ProjectName, branchId, branch.BranchName, file.Id, version);
		}

		public IEnumerable<DeployBuild> GetBuildList()
		{
			return this._buildRepository.GetBuildList();
		}

	}
}
