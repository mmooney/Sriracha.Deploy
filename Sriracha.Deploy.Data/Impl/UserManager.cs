using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Helpers;

namespace Sriracha.Deploy.Data.Impl
{
    public class UserManager : IUserManager
    {
        private readonly IMembershipRepository _membershipRepository;

        public UserManager(IMembershipRepository membershipRepository)
        {
            _membershipRepository = DIHelper.VerifyParameter(membershipRepository);
        }

        public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, List<string> userNameList = null)
        {
            return _membershipRepository.GetUserList(listOptions, userNameList);
        }

        public SrirachaUser GetUser(string userId)
        {
            return _membershipRepository.GetUser(userId);
        }

        public SrirachaUser GetUserByUserName(string userName)
        {
            return _membershipRepository.GetUserByUserName(userName);
        }

        public SrirachaUser TryGetUserByUserName(string userName)
        {
            return _membershipRepository.TryGetUserByUserName(userName);
        }

        public SrirachaUser CreateUser(string userName, string emailAddress, string password)
        {
            var user = new SrirachaUser
            {
                UserName = userName,
                EmailAddress = emailAddress
            };
            if(!string.IsNullOrEmpty(password))
            {
                user.EncryptedPassword = this.GetEncryptedPassword(userName, password);
            }
            return _membershipRepository.CreateUser(user);
        }

        private string GetEncryptedPassword(string userName, string password)
        {
            return Crypto.HashPassword(userName.ToLower() + password);
        }

        public SrirachaUser UpdateUser(string userId, string userName, string emailAddress, string password)
        {
            var user = _membershipRepository.GetUser(userId);
            user.UserName = userName;
            user.EmailAddress = emailAddress;
            if(!string.IsNullOrEmpty(password))
            {
                user.EncryptedPassword = GetEncryptedPassword(userName, password);
            }
            return _membershipRepository.UpdateUser(user);
        }


        public SrirachaUser DeleteUser(string userId)
        {
            return _membershipRepository.DeleteUser(userId);
        }
    }
}
