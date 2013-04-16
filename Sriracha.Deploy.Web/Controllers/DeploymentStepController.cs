using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Web.Models;

namespace Sriracha.Deploy.Web.Controllers
{
    public class DeploymentStepController : ApiController
    {
		private readonly IProjectManager _projectManager;

		public DeploymentStepController(IProjectManager projectManager)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

        // GET api/deploymentstep
		public IEnumerable<JSDeploymentStep> Get()
        {
			throw new NotImplementedException();
        }

        // GET api/deploymentstep/5
		public JSDeploymentStep Get(int id)
        {
			throw new NotImplementedException();
		}

        // POST api/deploymentstep
        public void Post(string projectId, string componentId, string deploymentStepId, JSDeploymentStep value)
        {
			if(string.IsNullOrEmpty(deploymentStepId))
			{
				_projectManager.CreateDeploymentStep(projectId, componentId, value.StepName, value.TaskType, value.TaskOptions);
			}
			else 
			{
				_projectManager.UpdateDeploymentStep(projectId, componentId, deploymentStepId, value.StepName, value.TaskType, value.TaskOptions);
			}
        }

        // PUT api/deploymentstep/5
        public void Put(int id, JSDeploymentStep value)
        {
        }

        // DELETE api/deploymentstep/5
        public void Delete(int id) 
        {
        }
    }
}
