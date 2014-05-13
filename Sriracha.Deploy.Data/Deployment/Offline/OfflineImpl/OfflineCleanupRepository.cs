using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineCleanupRepository : ICleanupRepository
    {
        public CleanupTaskData CreateCleanupTask(string machineName, EnumCleanupTaskType taskType, string folderPath, int ageMinutes)
        {
            //throw new NotImplementedException();
            return new CleanupTaskData();
        }

        public CleanupTaskData PopNextFolderCleanupTask(string machineName)
        {
            throw new NotImplementedException();
        }

        public CleanupTaskData MarkItemSuccessful(string taskId)
        {
            throw new NotImplementedException();
        }

        public CleanupTaskData MarkItemFailed(string taskId, Exception err)
        {
            throw new NotImplementedException();
        }


        public CleanupTaskData GetCleanupTask(string p)
        {
            throw new NotImplementedException();
        }
    }
}
