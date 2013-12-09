using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface ICleanupRepository
	{
		CleanupTaskData CreateCleanupTask(string machineName, EnumCleanupTaskType taskType, string folderPath, int ageMinutes);
		CleanupTaskData PopNextFolderCleanupTask(string machineName);
		void MarkItemSuccessful(string taskId);
		void MarkItemFailed(string taskId, Exception err);
	}
}
