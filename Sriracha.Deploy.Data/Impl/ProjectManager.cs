using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class ProjectManager : IProjectManager
	{
		private IProjectRepository ProjectRepository { get; set; }

		public ProjectManager(IProjectRepository projectRepository)
		{
			this.ProjectRepository = projectRepository;
		}

		public DeployProject CreateProject(string projectName)
		{
			return this.ProjectRepository.CreateProject(projectName);
		}

		public DeployProject GetProject(string projectId)
		{
			return this.ProjectRepository.GetProject(projectId);
		}

		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			return this.ProjectRepository.CreateBranch(projectId, branchName);
		}


		public IEnumerable<DeployProject> GetProjectList()
		{
			return this.ProjectRepository.GetProjectList();
		}


		public void DeleteProject(string projectId)
		{
			this.ProjectRepository.DeleteProject(projectId);
		}


		public void UpdateProject(string projectId, string projectName)
		{
			this.ProjectRepository.UpdateProject(projectId, projectName);
		}
	}
}
