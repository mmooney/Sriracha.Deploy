using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using NLog;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenCleanupRepository : ICleanupRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;
		private readonly Logger _logger;

		public RavenCleanupRepository(IDocumentSession documentSession, IUserIdentity userIdentity, Logger logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public CleanupTaskData CreateCleanupTask(string machineName, EnumCleanupTaskType taskType, string folderPath, int ageMinutes)
		{
            if(string.IsNullOrEmpty(machineName))
            {
                throw new ArgumentNullException("Missing Machine Name");
            }
            if(string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException("Missing Folder Path");
            }
            //if(ageMinutes <= 0)
            //{
            //    throw new ArgumentNullException("Missing Age Minutes");
            //}
			var task = new CleanupTaskData
			{
				Id = Guid.NewGuid().ToString(),
				TaskType = taskType,
				MachineName = machineName,
				FolderPath = folderPath,
				AgeMinutes = ageMinutes,
				Status = EnumQueueStatus.New,
				TargetCleanupDateTimeUtc = DateTime.UtcNow.AddMinutes(ageMinutes),
				CreatedByUserName = _userIdentity.UserName,
				CreatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow
			};
			return _documentSession.StoreSaveEvict(task);
		}


		public CleanupTaskData PopNextFolderCleanupTask(string machineName)
		{
            if(string.IsNullOrEmpty(machineName))
            {
                throw new ArgumentNullException("Missing Machine Name");
            }
			string itemId = null;
			using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				this._logger.Trace("Checking for next cleanup task");
				var tempItem = this._documentSession.QueryNoCache<CleanupTaskData>()
										.Customize(i => i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(30)))
										.OrderBy(i => i.TargetCleanupDateTimeUtc)
										.Where(i => i.Status == EnumQueueStatus.New 
												&& i.MachineName == machineName 
												&& i.TaskType == EnumCleanupTaskType.Folder
												&& i.TargetCleanupDateTimeUtc < DateTime.UtcNow)
										.FirstOrDefault();
				if (tempItem == null)
				{
					this._logger.Trace("No pending cleanup task");
					return null;
				}

				var reloadedItem = this._documentSession.LoadEnsure<CleanupTaskData>(tempItem.Id);
				if (reloadedItem.Status != EnumQueueStatus.New)
				{
					this._logger.Warn("Stale cleanup task found, actual status: " + reloadedItem.Status.ToString());
					return null;
				}

				reloadedItem.Status = EnumQueueStatus.InProcess;
				reloadedItem.StartedDateTimeUtc = DateTime.UtcNow;
                reloadedItem.UpdatedByUserName = _userIdentity.UserName;
                reloadedItem.UpdatedDateTimeUtc = DateTime.UtcNow;
				itemId = reloadedItem.Id;
				this._documentSession.SaveEvict(reloadedItem);

				transaction.Complete();
			}
			if (string.IsNullOrEmpty(itemId))
			{
				return null;
			}
			else
			{
				return _documentSession.LoadEnsureNoCache<CleanupTaskData>(itemId);
			}
		}

        public CleanupTaskData MarkItemSuccessful(string taskId)
		{
			var item = _documentSession.LoadEnsure<CleanupTaskData>(taskId);
			item.Status = EnumQueueStatus.Completed;
			item.CompletedDateTimeUtc = DateTime.UtcNow;
            item.UpdatedByUserName = _userIdentity.UserName;
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
            return _documentSession.SaveEvict(item);
		}

        public CleanupTaskData MarkItemFailed(string taskId, Exception err)
		{
			var item = _documentSession.LoadEnsure<CleanupTaskData>(taskId);
			item.Status = EnumQueueStatus.Error;
            if(err != null)
            {
    			item.ErrorDetails = err.ToString();
            }
			item.CompletedDateTimeUtc = DateTime.UtcNow;
            item.UpdatedByUserName = _userIdentity.UserName;
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
            return _documentSession.SaveEvict(item);
        }


        public CleanupTaskData GetCleanupTask(string id)
        {
            return _documentSession.LoadEnsureNoCache<CleanupTaskData>(id);
        }
    }
}
