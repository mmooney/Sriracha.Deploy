using MMDB.Shared;
using Newtonsoft.Json;
using PagedList;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerDeployRepository : IDeployRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        [PetaPoco.TableName("DeployBatchRequest")]
        private class DBDeployBatchRequest
        {
            public string ID { get; set; }
            public DateTime SubmittedDateTimeUtc { get; set; }
            public string SubmittedByUserName { get; set; }
            public string ItemListJson { get; set; }
            public EnumDeployStatus EnumDeployStatusID { get; set; }
            public DateTime? StartedDateTimeUtc { get; set; }
            public DateTime? CompleteDateTimeUtc { get; set; }
            public string ErrorDetails { get; set; }
            public string LastStatusMessage { get; set; }
            public string DeploymentLabel { get; set; }
            public bool CancelRequested { get; set; }
            public string CancelMessage { get; set; }

            public bool ResumeRequested { get; set; }
            public string ResumeMessage { get; set; }

            public string MessageListJson { get; set; }

            public DateTime CreatedDateTimeUtc { get; set; }
            public string CreatedByUserName { get; set; }
            public DateTime UpdatedDateTimeUtc { get; set; }
            public string UpdatedByUserName { get; set; }


            public DeployBatchRequest ToDto()
            {
                return new DeployBatchRequest
                {
                    Id = this.ID,
                    SubmittedDateTimeUtc = this.SubmittedDateTimeUtc, 
                    SubmittedByUserName = this.SubmittedByUserName,
                    CancelMessage = this.CancelMessage,
                    CancelRequested = this.CancelRequested,
                    CompleteDateTimeUtc = this.CompleteDateTimeUtc,
                    CreatedByUserName = this.CreatedByUserName,
                    CreatedDateTimeUtc = this.CreatedDateTimeUtc,
                    DeploymentLabel = this.DeploymentLabel,
                    ErrorDetails = this.ErrorDetails,
                    ItemList = this.FromJson<List<DeployBatchRequestItem>>(this.ItemListJson),
                    LastStatusMessage = this.LastStatusMessage,
                    MessageList = this.FromJson<List<string>>(this.MessageListJson),
                    ResumeMessage = this.ResumeMessage,
                    ResumeRequested = this.ResumeRequested,
                    StartedDateTimeUtc = this.StartedDateTimeUtc,
                    Status = this.EnumDeployStatusID,
                    UpdatedByUserName = this.UpdatedByUserName,
                    UpdatedDateTimeUtc = this.UpdatedDateTimeUtc
                };
            }

            private T FromJson<T>(string json) where T:class
            {
                if(string.IsNullOrEmpty(json))
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
        }

        private void VerifyExists(string deployBatchRequestId)
        {
            if(string.IsNullOrEmpty(deployBatchRequestId))
            {
                throw new ArgumentNullException("deployBatchRequestId");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployBatchRequest WHERE ID=@0", deployBatchRequestId);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployBatchRequest), "ID", deployBatchRequestId);
                }
            }
        }

        public SqlServerDeployRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public PagedSortedList<DeployBatchRequest> GetBatchRequestList(ListOptions listOptions)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 10, 1, "SubmittedDateTimeUtc", false);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var list = db.PageAndSort<DBDeployBatchRequest>(listOptions, PetaPoco.Sql.Builder.Append(""));
                return list.Cast(i=>i.ToDto());
            }
        }

        public DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, EnumDeployStatus status, string deploymentLabel)
        {
            if(itemList == null || itemList.Count == 0)
            {
                throw new ArgumentNullException("itemList");
            }
            foreach(var item in itemList)
            {
                //We're creating data, make sure we give the children new IDs, otherwise copying gets all busted up.
                //if(string.IsNullOrEmpty(item.Id))
                //{
                    item.Id = Guid.NewGuid().ToString();
                //}
            }
            if(status == EnumDeployStatus.Unknown)
            {
                status = EnumDeployStatus.NotStarted;
            }
            string message = string.Format("{0} created deployment request with status of {1} at {2} UTC.", _userIdentity.UserName, EnumHelper.GetDisplayValue(status), DateTime.UtcNow);
            var dbItem = new DBDeployBatchRequest
            {
                ID = Guid.NewGuid().ToString(),
                SubmittedByUserName = _userIdentity.UserName,
                SubmittedDateTimeUtc = DateTime.UtcNow,
                DeploymentLabel = deploymentLabel,
                EnumDeployStatusID = status,
                ItemListJson = itemList.ToJson(),
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                LastStatusMessage = message,
                MessageListJson = (new List<string> { message }).ToJson(),
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("DeployBatchRequest", "ID", false, dbItem);
            }
            return this.GetBatchRequest(dbItem.ID);
        }

        public DeployBatchRequest PopNextBatchDeployment()
        {
            var sql = PetaPoco.Sql.Builder
                        .Append("DECLARE @@id AS NVARCHAR(50);")
                        .Append("SET @@id = (SELECT TOP 1 ID FROM DeployBatchRequest WHERE EnumDeployStatusID=@0 OR (ResumeRequested=1 AND (EnumDeployStatusID=@1 OR EnumDeployStatusID=@2)) ORDER BY SubmittedDateTimeUtc ASC);", EnumDeployStatus.NotStarted, EnumDeployStatus.Error, EnumDeployStatus.Cancelled)
                        .Append("IF(@@id IS NOT NULL) UPDATE DeployBatchRequest SET EnumDeployStatusID=@0, StartedDateTimeUtc=GETUTCDATE(), UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@1 WHERE ID=@@id;", EnumDeployStatus.InProcess, _userIdentity.UserName)
                        .Append("SELECT @@id");
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var id = db.ExecuteScalar<string>(sql);
                if (!string.IsNullOrEmpty(id))
                {
                    return GetBatchRequest(id);
                }
                else
                {
                    return null;
                }
            }
        }

        public DeployBatchRequest GetBatchRequest(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<DBDeployBatchRequest>("FROM DeployBatchRequest WHERE ID=@0", id);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployBatchRequest), "ID", id);
                }
                return item.ToDto();
            }
        }

        public DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory = true)
        {
            var existingItem = GetBatchRequest(deployBatchRequestId);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployBatchRequest")
                        .Append("SET EnumDeployStatusID = @0, LastStatusMessage=@1, UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@2", status, statusMessage, _userIdentity.UserName);
            if(err != null)
            {
                sql = sql.Append(", ErrorDetails=@0",err.ToString());
            }
            if(addToMessageHistory && !string.IsNullOrEmpty(statusMessage))
            {
                existingItem.MessageList.Add(statusMessage);
                sql = sql.Append(", MessageListJson=@0", existingItem.MessageList.ToJson());
            }
            switch (status)
            {
                case EnumDeployStatus.Success:
                case EnumDeployStatus.Error:
                    sql = sql.Append(", CompleteDateTimeUtc=GETUTCDATE()");
                    sql = sql.Append(", CancelRequested=0");
                    break;
                case EnumDeployStatus.InProcess:
                    sql = sql.Append(", ResumeRequested=0");
                    break;
                case EnumDeployStatus.Cancelled:
                    sql = sql.Append(", CancelRequested=0");
                    break;
            }
            sql = sql.Append("WHERE ID=@0", deployBatchRequestId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return this.GetBatchRequest(deployBatchRequestId);
        }

        public PagedSortedList<DeployBatchRequest> GetDeployQueue(ListOptions listOptions, List<EnumDeployStatus> statusList = null, List<string> environmentIds = null, bool includeResumeRequested = true)
        {
            if (statusList == null || statusList.Count == 0)
            {
                statusList = new List<EnumDeployStatus> { EnumDeployStatus.NotStarted, EnumDeployStatus.InProcess };
            }
            listOptions = ListOptions.SetDefaults(listOptions, 10, 1, "SubmittedDateTimeUtc", false);
            var sql = PetaPoco.Sql.Builder
                        .Append("WHERE (EnumDeployStatusID IN (@statusList)", new {statusList=statusList})
                            .Append("OR (ResumeRequested=1 AND (EnumDeployStatusID=@0 OR EnumDeployStatusID=@1)))", EnumDeployStatus.Error, EnumDeployStatus.Cancelled);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var list = db.PageAndSort<DBDeployBatchRequest>(listOptions, sql);
                return list.Cast(i=>i.ToDto());
            }
        }

        public DeployBatchRequest RequeueDeployment(string deployBatchRequestId, EnumDeployStatus status, string userMessage)
        {
            var existingItem = GetBatchRequest(deployBatchRequestId);
            string statusMessage = string.Format("{0} requested deployment to be requeued at {1} UTC", _userIdentity.UserName, DateTime.UtcNow);
            if (!string.IsNullOrEmpty(userMessage))
            {
                statusMessage += ". Notes: " + userMessage;
            }
            existingItem.MessageList.Add(statusMessage);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployBatchRequest")
                        .Append("SET EnumDeployStatusID=@0, StartedDateTimeUtc=NULL,", status)
                        .Append("LastStatusMessage=@0, MessageListJson=@1,", statusMessage, existingItem.MessageList.ToJson())
                        .Append("UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@0", _userIdentity.UserName)
                        .Append("WHERE ID=@0", deployBatchRequestId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return GetBatchRequest(deployBatchRequestId);
        }

        public DeployBatchRequest SetCancelRequested(string deployBatchRequestId, string userMessage)
        {
            var existingItem = GetBatchRequest(deployBatchRequestId);
            string statusMessage = string.Format("{0} requested deployment to be cancelled at {1} UTC", _userIdentity.UserName, DateTime.UtcNow);
            if (!string.IsNullOrEmpty(userMessage))
            {
                statusMessage += ". Notes: " + userMessage;
            }
            existingItem.MessageList.Add(statusMessage);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployBatchRequest")
                        .Append("SET CancelRequested=1,CancelMessage=@0,", userMessage)
                        .Append("LastStatusMessage=@0, MessageListJson=@1,", statusMessage, existingItem.MessageList.ToJson())
                        .Append("UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@0", _userIdentity.UserName)
                        .Append("WHERE ID=@0", deployBatchRequestId);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return GetBatchRequest(deployBatchRequestId);
        }

        public DeployBatchRequest SetResumeRequested(string deployBatchRequestId, string userMessage)
        {
            var existingItem = GetBatchRequest(deployBatchRequestId);
            string statusMessage = string.Format("{0} requested deployment to be resumed at {1} UTC", _userIdentity.UserName, DateTime.UtcNow);
            if (!string.IsNullOrEmpty(userMessage))
            {
                statusMessage += ". Notes: " + userMessage;
            }
            existingItem.MessageList.Add(statusMessage);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployBatchRequest")
                        .Append("SET ResumeRequested=1, ResumeMessage=@0,", userMessage)
                        .Append("LastStatusMessage=@0, MessageListJson=@1,", statusMessage, existingItem.MessageList.ToJson())
                        .Append("UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@0", _userIdentity.UserName)
                        .Append("WHERE ID=@0", deployBatchRequestId);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return GetBatchRequest(deployBatchRequestId);
        }
    }
}
