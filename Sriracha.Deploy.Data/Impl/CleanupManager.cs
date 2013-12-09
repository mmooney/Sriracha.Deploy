using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class CleanupManager : ICleanupManager
	{
		private readonly ICleanupRepository _cleanupRepository;

		public CleanupManager(ICleanupRepository cleanupRepository)
		{
			_cleanupRepository = DIHelper.VerifyParameter(cleanupRepository);
		}

		public CleanupTaskData QueueFolderForCleanup(string machineName, string folderPath, int ageMinutes)
		{
			return _cleanupRepository.CreateCleanupTask(machineName, EnumCleanupTaskType.Folder, folderPath, ageMinutes);
		}


		public CleanupTaskData PopNextFolderCleanupTask(string machineName)
		{
			return _cleanupRepository.PopNextFolderCleanupTask(machineName);
		}

		public void CleanupFolder(CleanupTaskData item)
		{
			if(Directory.Exists(item.FolderPath))
			{
				Directory.Delete(item.FolderPath, true);
			}
		}

		public void MarkItemSuccessful(CleanupTaskData item)
		{
			_cleanupRepository.MarkItemSuccessful(item.Id);
		}

		public void MarkItemFailed(CleanupTaskData item, Exception err)
		{
			_cleanupRepository.MarkItemFailed(item.Id, err);
		}
	}
}
