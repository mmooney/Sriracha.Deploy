using MMDB.Shared;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services
{
    public class TaskOptionsViewService : Service
    {
		private readonly ITaskManager _taskManager;

        public TaskOptionsViewService(ITaskManager taskManager)
        {
			_taskManager = taskManager;
        }

        public object Get(TaskOptionsViewRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.TaskTypeName))
            {
                throw new ArgumentNullException("request.taskTypeName is null");
            }
            return _taskManager.TryGetTaskOptionsView(request.TaskTypeName);
        }
    }
}