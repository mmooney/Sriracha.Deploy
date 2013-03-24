using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Controllers
{
    public class ProjectController : ApiController
    {
        // GET api/project
        public IEnumerable<DeployProject> Get()
        {
            return new DeployProject[]
			{
				new DeployProject { Id="1", ProjectName="test project 1" },
				new DeployProject { Id="2", ProjectName="test project 2" }
			};
        }

        // GET api/project/5
        public DeployProject Get(int id)
        {
			return new DeployProject { Id=id.ToString(), ProjectName="test project " + id.ToString() };
        }

        // POST api/project
        public void Post([FromBody]string value)
        {
        }

        // PUT api/project/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/project/5
        public void Delete(int id)
        {
        }
    }
}
