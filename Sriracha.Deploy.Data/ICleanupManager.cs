﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface ICleanupManager
	{
		CleanupTaskData QueueFolderForCleanup(string machineName, string folderPath, int ageMinutes);
		CleanupTaskData PopNextFolderCleanupTask(string machineName);
		void CleanupFolder(CleanupTaskData item);
		void MarkItemSuccessful(CleanupTaskData item);
		void MarkItemFailed(CleanupTaskData item, Exception err);
	}
}
