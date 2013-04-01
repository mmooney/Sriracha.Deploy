﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenProjectRepository : IProjectRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenProjectRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public IEnumerable<DeployProject> GetProjectList()
		{
			return _documentSession.Query<DeployProject>();
		}

		public DeployProject CreateProject(string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var project = new DeployProject
			{
				Id = Guid.NewGuid().ToString(),
				ProjectName = projectName
			};
			_documentSession.Store(project);
			_documentSession.SaveChanges();
			return project;
		}

		public DeployProject GetProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			var item = _documentSession.Load<DeployProject>(projectId);
			if(item == null)
			{
				throw new KeyNotFoundException("Project not found: " + projectId);
			}
			return item;
		}

		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			if(string.IsNullOrEmpty(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			var project = this.GetProject(projectId);
			var branch = new DeployProjectBranch 
			{
				Id = Guid.NewGuid().ToString(),
				BranchName = branchName,
				ProjectId = projectId 
			};
			project.BranchList.Add(branch);
			this._documentSession.SaveChanges();
			return branch;
		}


		public void DeleteProject(string projectId)
		{
			var item = this.GetProject(projectId);
			this._documentSession.Delete(item);
			this._documentSession.SaveChanges();
		}


		public DeployProject UpdateProject(string projectId, string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var item = this.GetProject(projectId);
			item.ProjectName = projectName;
			this._documentSession.SaveChanges();
			return item;
		}


		public DeployComponent CreateComponent(string projectId, string componentName)
		{
			var project = GetProject(projectId);
			var item = new DeployComponent
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ComponentName = componentName
			};
			project.ComponentList.Add(item);
			this._documentSession.SaveChanges();
			return item;
		}

		public DeployComponent UpdateComponent(string projectId, string componentId, string componentName)
		{
			var project = GetProject(projectId);
			var item = project.ComponentList.Single(i=>i.Id == componentId);
			item.ComponentName = componentName;
			this._documentSession.SaveChanges();
			return item;
		}
	}
}
