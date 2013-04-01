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
    public class ComponentController : ApiController
    {
		private readonly IProjectManager _projectManager;

		public ComponentController(IProjectManager projectManager)
		{
			this._projectManager = DIHelper.VerifyParameter(projectManager);
		}
        // GET api/component
        public IEnumerable<DeployComponent> Get(string projectId)
        {
			return _projectManager.GetProject(projectId).ComponentList;
        }

        // POST api/component
        public void Post(string projectId, DeployComponent component)
        {
			if(string.IsNullOrEmpty(component.Id))
			{
				_projectManager.CreateComponent(projectId, component.ComponentName);
			}
			else 
			{
				_projectManager.UpdateComponent(projectId, component.Id, component.ComponentName);
			}
        }

        // PUT api/component/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/component/5
        public void Delete(int id)
        {
        }
    }
}
