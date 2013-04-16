using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Web.Services
{
	public class TaskMetadataService : Service
	{
		private readonly ITaskManager _taskManager;

		public TaskMetadataService(ITaskManager taskManager)
		{
			_taskManager = taskManager;
		}

		public object Get(TaskMetadata x) 
		{
			return _taskManager.GetAvailableTaskList();
		}
	}
}