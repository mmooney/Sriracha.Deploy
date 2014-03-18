using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data
{
	public interface ITaskManager
	{
		List<TaskMetadata> GetAvailableTaskList();

        string GetTaskOptionsView(string taskTypeName);
    }
}
