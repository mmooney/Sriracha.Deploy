using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Web.Controllers
{
    public class TaskMetadataController : ApiController
    {
		private readonly ITaskManager _taskManager;

		public TaskMetadataController(ITaskManager taskManager)
		{
			this._taskManager = DIHelper.VerifyParameter(taskManager);
		}

        // GET api/taskmetadata
        public IEnumerable<TaskMetadata> Get()
        {
            return this._taskManager.GetAvailableTaskList();
        }

        // GET api/taskmetadata/5
        public TaskMetadata Get(string id)
        {
			throw new NotImplementedException();
		}

        // POST api/taskmetadata
        public void Post([FromBody]string value)
        {
			throw new NotSupportedException();
		}

        // PUT api/taskmetadata/5
        public void Put(int id, [FromBody]string value)
        {
			throw new NotSupportedException();
		}

        // DELETE api/taskmetadata/5
        public void Delete(int id)
        {
			throw new NotSupportedException();
		}
    }
}
