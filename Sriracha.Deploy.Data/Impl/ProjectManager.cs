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
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			return this.ProjectRepository.CreateProject(projectName);
		}

		public DeployProject GetProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			var item = this.ProjectRepository.GetProject(projectId);
			if(item == null)
			{
				throw new KeyNotFoundException("No project found for ID: " + projectId);
			}
			return item;
		}

		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			if(string.IsNullOrEmpty(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			return this.ProjectRepository.CreateBranch(projectId, branchName);
		}


		public IEnumerable<DeployProject> GetProjectList()
		{
			return this.ProjectRepository.GetProjectList();
		}


		public void DeleteProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			this.ProjectRepository.DeleteProject(projectId);
		}


		public void UpdateProject(string projectId, string projectName)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			if(string.IsNullOrEmpty(projectName)) 
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			this.ProjectRepository.UpdateProject(projectId, projectName);
		}
	}
}
