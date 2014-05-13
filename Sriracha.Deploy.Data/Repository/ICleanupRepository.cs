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
        CleanupTaskData GetCleanupTask(string taskId);
        CleanupTaskData PopNextFolderCleanupTask(string machineName);
		CleanupTaskData MarkItemSuccessful(string taskId);
        CleanupTaskData MarkItemFailed(string taskId, Exception err);

    }
}
