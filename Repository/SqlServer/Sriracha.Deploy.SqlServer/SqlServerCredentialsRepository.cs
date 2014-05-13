using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Credentials;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerCredentialsRepository : ICredentialsRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerCredentialsRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private PetaPoco.Sql GetBaseQuery()
        {
            return PetaPoco.Sql.Builder
                    .Append("SELECT ID, Domain, UserName, EncryptedPassword, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                    .Append("FROM DeployCredential");
        }

        private void CheckForDuplicate(string currentId, string domain, string userName)
        {
            var sql = PetaPoco.Sql.Builder
                        .Append("SELECT COUNT(*) FROM DeployCredential WHERE Domain=@0 AND UserName=@1", domain, userName);
            if(!string.IsNullOrEmpty(currentId))
            {
                sql = sql.Append("AND ID != @0", currentId);
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                int count = db.ExecuteScalar<int>(sql);
                if(count > 0)
                {
                    throw new ArgumentException(string.Format("UserName already exists: {0}\\{1}", domain, userName));
                }
            }
        }

        private void VerifyExists(string credentialsId)
        {
            if(string.IsNullOrEmpty(credentialsId))
            {
                throw new ArgumentNullException("credentialsId");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployCredential WHERE ID=@0", credentialsId);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployCredentials), "ID", credentialsId);
                }
            }
        }

        public PagedSortedList<DeployCredentials> GetCredentialsList(ListOptions listOptions)
        {
            var sql = GetBaseQuery();
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UserName", true);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                return db.PageAndSort<DeployCredentials>(listOptions, sql);
            }
        }


        public DeployCredentials GetCredentials(string credentialsId)
        {
            if(string.IsNullOrEmpty(credentialsId))
            {
                throw new ArgumentNullException("credentialsId");
            }
            var sql = GetBaseQuery();
            sql = sql.Append("WHERE ID=@0", credentialsId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<DeployCredentials>(sql);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployCredentials), "ID", credentialsId);
                }
                return item;
            }
        }

        public DeployCredentials CreateCredentials(string domain, string userName, string encrytpedPassword)
        {
            if(string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }
            if(string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if(string.IsNullOrEmpty(encrytpedPassword))
            {
                throw new ArgumentNullException("encryptedPassword");
            }
            CheckForDuplicate(null, domain, userName);
            var item = new DeployCredentials
            {
                Id = Guid.NewGuid().ToString(),
                Domain = domain,
                UserName = userName,
                EncryptedPassword = encrytpedPassword,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            var sql = PetaPoco.Sql.Builder
                        .Append("INSERT INTO DeployCredential (ID, Domain, UserName, EncryptedPassword, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                        .Append("VALUES (@Id, @Domain, @UserName, @EncryptedPassword, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", item);
            using(var db = _sqlConnectionInfo.GetDB())
            {  
                db.Execute(sql);
            }
            return this.GetCredentials(item.Id);
        }

        public DeployCredentials UpdateCredentials(string credentialsId, string domain, string userName, string encrytpedPassword)
        {
            if(string.IsNullOrEmpty(domain))
            { 
                throw new ArgumentNullException("domain");
            }
            if(string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if(string.IsNullOrEmpty(encrytpedPassword))
            {
                throw new ArgumentNullException("encrytpedPassword");
            }
            VerifyExists(credentialsId);
            CheckForDuplicate(credentialsId, domain, userName);
            var item = new DeployCredentials
            {
                Id = credentialsId,
                Domain = domain,
                UserName = userName,
                EncryptedPassword = encrytpedPassword,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployCredential")
                        .Append("SET Domain = @Domain, UserName=@UserName, EncryptedPassword=@EncryptedPassword, UpdatedByUserName=@UpdatedByUserName, UpdatedDateTimeUtc=@UpdatedDateTimeUtc", item)
                        .Append("WHERE ID=@0", credentialsId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return this.GetCredentials(credentialsId);
        }

        public DeployCredentials DeleteCredentials(string credentialsId)
        {
            var itemToDelete = this.GetCredentials(credentialsId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("DELETE FROM DeployCredential WHERE ID=@0", credentialsId);
            }
            return itemToDelete;
        }
    }
}
