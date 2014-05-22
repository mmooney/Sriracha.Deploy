using MMDB.Shared;
using Newtonsoft.Json;
using PagedList;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerMembershipRepository : IMembershipRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        [PetaPoco.TableName("SrirachaUser")]
        private class SqlSrirachaUser : BaseDto
        {
            public string UserName { get; set; }
            public Guid UserGuid { get; set; }
            public string EmailAddress { get; set; }
            public string EncryptedPassword { get; set; }
            public DateTime? LastPasswordChangedDateTimeUtc { get; set; }

            public string PasswordQuestion { get; set; }
            public string PasswordAnswer { get; set; }

            public DateTime? LastLockoutDateTimeUtc { get; set; }
            public DateTime? LastLoginDateDateTimeUtc { get; set; }


            public bool LockedIndicator { get; set; }

            public bool MustChangePasswordIndicator { get; set; }
            public DateTime LastActivityDateTimeUtc { get; set; }

            public string ProjectNotificationItemListJson { get; set; }

            public SrirachaUser ToDto()
            {
                var item = AutoMapper.Mapper.Map(this, new SrirachaUser());
                if(!string.IsNullOrEmpty(this.ProjectNotificationItemListJson))
                {
                    item.ProjectNotificationItemList = JsonConvert.DeserializeObject<List<ProjectNotificationItem>>(this.ProjectNotificationItemListJson);
                }
                return item;
            }

            public static SqlSrirachaUser FromDto(SrirachaUser user)
            {
                var dbItem = AutoMapper.Mapper.Map(user, new SqlSrirachaUser());
                if (user.ProjectNotificationItemList != null)
                {
                    dbItem.ProjectNotificationItemListJson = user.ProjectNotificationItemList.ToJson();
                }
                return dbItem;
            }
        }

        static SqlServerMembershipRepository()
        {
            AutoMapper.Mapper.CreateMap<SqlSrirachaUser, SrirachaUser>();
            AutoMapper.Mapper.CreateMap<SrirachaUser, SqlSrirachaUser>();
        }

        public SqlServerMembershipRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        //private string FormatId(string userName)
        //{
        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        throw new ArgumentNullException("userName");
        //    }
        //    return "SrirachaUser_" + userName.Replace('\\', '_');
        //}

        public SrirachaUser CreateUser(SrirachaUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException("user");
            }
            if(string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentNullException("user.UserName");
            }
            if(UserNameExists(user.UserName))
            {
                throw new ArgumentException("UserName already exists: " + user.UserName);
            }
            var dbItem = SqlSrirachaUser.FromDto(user);
            dbItem.Id = Guid.NewGuid().ToString();
            if(user.UserGuid == Guid.Empty)
            {
                dbItem.UserGuid = Guid.NewGuid();
            }
            dbItem.SetCreatedFields(this._userIdentity.UserName);
            //dbItem.Id = FormatId(user.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("SrirachaUser", "ID", false, dbItem);
            }
            return this.GetUser(dbItem.Id);
        }

        public SrirachaUser UpdateUser(SrirachaUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException("user");
            }
            if(string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentNullException("user.UserName");
            }
            var existingUser = GetUser(user.Id);
            var dbItem = SqlSrirachaUser.FromDto(user);
            dbItem.CreatedByUserName = existingUser.CreatedByUserName;
            dbItem.CreatedDateTimeUtc = existingUser.CreatedDateTimeUtc;
            dbItem.SetUpdatedFields(this._userIdentity.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("SrirachaUser", "ID", dbItem, dbItem.Id);
            }
            return this.GetUser(dbItem.Id);
        }

        public SrirachaUser DeleteUser(string userId)
        {
            var itemToDelete = this.GetUser(userId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("DELETE FROM SrirachaUser WHERE ID=@0", userId);
            }
            return itemToDelete;
        }

        public SrirachaUser GetUser(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.SingleOrDefault<SqlSrirachaUser>("WHERE ID=@0", userId);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(SrirachaUser),"Id", userId);
                }
                return item.ToDto();
            }
        }

        public SrirachaUser GetUserByUserName(string userName)
        {
            var item = this.TryGetUserByUserName(userName);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(SrirachaUser), "UserName", userName);
            }
            return item;
        }

        public SrirachaUser TryGetUserByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.SingleOrDefault<SqlSrirachaUser>("WHERE UserName=@0", userName);
                if (item != null)
                {
                    return item.ToDto();
                }
                else 
                {
                    return null;
                }
            }
        }

        public SrirachaUser TryGetUserByUserGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
            {
                throw new ArgumentNullException("userGuid");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.SingleOrDefault<SqlSrirachaUser>("WHERE UserGuid=@0", userGuid);
                if (item != null)
                {
                    return item.ToDto();
                }
                else
                {
                    return null;
                }
            }
        }

        public SrirachaUser TryGetUserByEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException("emailAddress");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.SingleOrDefault<SqlSrirachaUser>("WHERE EmailAddress=@0", emailAddress);
                if (item != null)
                {
                    return item.ToDto();
                }
                else
                {
                    return null;
                }
            }
        }

        public bool UserNameExists(string userName)
        {
            if(string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM SrirachaUser WHERE UserName=@0", userName);
                return (count != 0);
            }
        }

        public bool EmailAddressExists(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM SrirachaUser WHERE EmailAddress=@0", email);
                return (count != 0);
            }
        }

        public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, List<string> userNameList = null, List<string> emailAddressList = null)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UserName", true);
            var sql = PetaPoco.Sql.Builder.Append("WHERE 0=0");
            if(userNameList != null && userNameList.Any())
            {
                sql = sql.Append("AND UserName IN (@userNameList)", new {userNameList});
            }
            if(emailAddressList != null && emailAddressList.Any())
            {
                sql = sql.Append("AND EmailAddress IN (@emailAddressList)", new {emailAddressList});
            }
            switch(listOptions.SortField)
            {
                case "UserName":
                case "EmailAddress":   
                    //Have a nice day!
                    break;
                default:
                    throw new UnrecognizedSortFieldException<SrirachaUser>(listOptions);
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var list = db.PageAndSort<SqlSrirachaUser>(listOptions, sql);
                var pagedList = new StaticPagedList<SrirachaUser>(list.Items.Select(i=>i.ToDto()), list.PageNumber, list.PageSize, list.TotalItemCount);
                return new PagedSortedList<SrirachaUser>(pagedList, list.SortField, list.SortAscending);
            }
        }

        public int GetUserCount(DateTime? lastActivityDateTimeUtc = null)
        {
            var sql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM SrirachaUser");
            if(lastActivityDateTimeUtc.HasValue)
            {
                sql = sql.Append("WHERE LastActivityDateTimeUtc>@0", lastActivityDateTimeUtc.Value);
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                return db.ExecuteScalar<int>(sql);
            }
        }
    }
}
