using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Controllers
{
    public class ProjectController : ApiController
    {
		private readonly IProjectManager _projectMananger;

		public ProjectController(IProjectManager projectManager)
		{
			_projectMananger = DIHelper.VerifyParameter(projectManager);	
		}

        // GET api/project
        public IEnumerable<DeployProject> Get()
        {
			return this._projectMananger.GetProjectList();
        }

        // GET api/project/5
        public DeployProject Get(string id)
        {
			return this._projectMananger.GetProject(id);
        }

        // POST api/project
        public void Post(DeployProject project)
        {
			if(string.IsNullOrEmpty(project.Id))
			{
				this._projectMananger.CreateProject(project.ProjectName);
			}
			else 
			{
				this._projectMananger.UpdateProject(project.Id, project.ProjectName);
			}
        }

        // PUT api/project/5
        public void Put(int id, DeployProject project)
        {
			throw new NotImplementedException();
        }

        // DELETE api/project/5
        public void Delete(string id)
        {
			this._projectMananger.DeleteProject(id);
        }
    }
}
