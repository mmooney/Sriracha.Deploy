using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerEmailQueueRepository : IEmailQueueRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        [PetaPoco.TableName("SrirachaEmailMessage")]
        private class SqlSrirachaEmailMessage : BaseDto
        {
            public string Subject { get; set; }
            public string DataObjectJson { get; set; }
            public string RazorView { get; set; }
            public EnumQueueStatus EnumQueueStatusID { get; set; }
            public DateTime? StartedDateTimeUtc { get; set; }
            public DateTime QueueDateTimeUtc { get; set; }
            public string EmailAddressListJson { get; set; }
            public string RecipientResultListJson { get; set; }

            public SrirachaEmailMessage ToDto()
            {
                return new SrirachaEmailMessage
                {
                    Id = this.Id,
                    Subject = this.Subject,
                    DataObject = FromJson(this.DataObjectJson),
                    RazorView = this.RazorView,
                    Status = this.EnumQueueStatusID,
                    StartedDateTimeUtc = this.StartedDateTimeUtc,
                    QueueDateTimeUtc = this.QueueDateTimeUtc, 
                    EmailAddressList = FromJson<List<string>>(this.EmailAddressListJson, new List<string>()),
                    RecipientResultList = FromJson<List<SrirachaEmailMessage.SrirachaEmailMessageRecipientResult>>(this.RecipientResultListJson, new List<SrirachaEmailMessage.SrirachaEmailMessageRecipientResult>()),
                    CreatedByUserName = this.CreatedByUserName,
                    CreatedDateTimeUtc = this.CreatedDateTimeUtc,
                    UpdatedByUserName = this.UpdatedByUserName,
                    UpdatedDateTimeUtc = this.UpdatedDateTimeUtc
                };
            }

            private object FromJson(string json)
            {
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject(json);
                }
            }

            private T FromJson<T>(string json, T defaultValue=default(T))
            {
                if(string.IsNullOrEmpty(json))
                {
                    return defaultValue;
                }
                else 
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
        }

        public SqlServerEmailQueueRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private void VerifyExists(string emailMessageId)
        {
            if (string.IsNullOrEmpty(emailMessageId))
            {
                throw new ArgumentNullException("emailMessageId");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM SrirachaEmailMessage WHERE ID=@0", emailMessageId);
                if (count == 0)
                {
                    throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "Id", emailMessageId);
                }
            }
        }

        public SrirachaEmailMessage CreateMessage(string subject, List<string> emailAddressList, object dataObject, string razorView)
        {
            if(string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException("subject");
            }
            if(emailAddressList == null || emailAddressList.Count == 0)
            {
                throw new ArgumentNullException("emailAddressList");
            }
            if(dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }
            if(string.IsNullOrEmpty(razorView))
            {
                throw new ArgumentNullException("razorView");
            }
            var dbItem = new SqlSrirachaEmailMessage
            {
                Id = Guid.NewGuid().ToString(),
                EmailAddressListJson = emailAddressList.ToJson(),
                DataObjectJson = dataObject.ToJson(),
                RazorView = razorView,
                QueueDateTimeUtc = DateTime.UtcNow,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                EnumQueueStatusID = EnumQueueStatus.New,
                Subject = subject
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("SrirachaEmailMessage", "ID", false, dbItem);
            }
            return this.GetMessage(dbItem.Id);
        }

        public SrirachaEmailMessage GetMessage(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var dbItem = db.FirstOrDefault<SqlSrirachaEmailMessage>("WHERE ID=@0", id);
                if(dbItem == null)
                {
                    throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "Id", id);
                }
                return dbItem.ToDto();
            }
        }

        public SrirachaEmailMessage PopNextMessage()
        {
            var sql = PetaPoco.Sql.Builder
                        .Append("DECLARE @@id AS NVARCHAR(50);")
                        .Append("SET @@id = (SELECT TOP 1 ID FROM SrirachaEmailMessage WHERE EnumQueueStatusID=@0 ORDER BY QueueDateTimeUtc ASC);", EnumQueueStatus.New)
                        .Append("IF(@@id IS NOT NULL) UPDATE SrirachaEmailMessage SET EnumQueueStatusID=@0, StartedDateTimeUtc=GETUTCDATE(), UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@1 WHERE ID=@@id;", EnumQueueStatus.InProcess, _userIdentity.UserName)
                        .Append("SELECT @@id");
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var id = db.ExecuteScalar<string>(sql);
                if (!string.IsNullOrEmpty(id))
                {
                    return GetMessage(id);
                }
                else
                {
                    return null;
                }
            }
        }

        public SrirachaEmailMessage UpdateMessageStatus(string emailMessageId, EnumQueueStatus status)
        {
            VerifyExists(emailMessageId);
            var dbItem = new SqlSrirachaEmailMessage
            {
                Id = emailMessageId,
                EnumQueueStatusID = status,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE SrirachaEmailMessage")
                        .Append("SET EnumQueueStatusID=@EnumQueueStatusID, UpdatedDateTimeUtc=@UpdatedDateTimeUtc, UpdatedByUserName=@UpdatedByUserName", dbItem)
                        .Append("WHERE ID=@Id",dbItem);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return this.GetMessage(emailMessageId);
        }

        public SrirachaEmailMessage AddReceipientResult(string emailMessageId, EnumQueueStatus status, string emailAddress, Exception err = null)
        {
            if(string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException("emailAddress");
            }
            var item = this.GetMessage(emailMessageId);
            if (item.EmailAddressList == null || !item.EmailAddressList.Contains(emailAddress))
            {
                throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "EmailAddress", emailAddress);
            }
            var newResult = new SrirachaEmailMessage.SrirachaEmailMessageRecipientResult
            {
                Id = Guid.NewGuid().ToString(),
                SrirachaEmailMessageId = emailMessageId,
                EmailAddress = emailAddress,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                Status = status,
                StatusDateTimeUtc = DateTime.UtcNow
            };
            if(err != null)
            {
                newResult.Details = err.ToString();
            }
            if(item.RecipientResultList == null)
            {
                item.RecipientResultList = new List<SrirachaEmailMessage.SrirachaEmailMessageRecipientResult>();
            }
            item.RecipientResultList.Add(newResult);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE SrirachaEmailMessage")
                        .Append("SET RecipientResultListJson=@0, UpdatedDateTimeUtc=@1, UpdatedByUserName=@2", item.RecipientResultList.ToJson(), DateTime.UtcNow, _userIdentity.UserName)
                        .Append("WHERE ID=@0", emailMessageId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return this.GetMessage(emailMessageId);
        }
    }
}
