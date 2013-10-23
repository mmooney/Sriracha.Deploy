using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Web.Security
{
	public class SrirachaMembershipProvider : MembershipProvider
	{
		public static string ProviderName = "SrirachaMembershipProvider";
		
		private string _applicationName = "Sriracha";
		public override string ApplicationName
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}

		private readonly IMembershipRepository _injectedRepository;

		public SrirachaMembershipProvider()
		{
			
		}
		
		public SrirachaMembershipProvider(IMembershipRepository repository)
		{
			_injectedRepository = repository;
		}

		private IMembershipRepository GetRepository()
		{
			if(_injectedRepository != null)
			{	
				return _injectedRepository;
			}
			else 
			{
				return DependencyResolver.Current.GetService<IMembershipRepository>();
			}
		}

		private void LogException(Exception err)
		{
			var logger = GetLogger();
			if(logger != null)
			{
				logger.ErrorException(err.Message, err);
			}
		}

		private void LogInfo(string message, params object[] args)
		{
			var logger = GetLogger();
			if(logger != null)
			{
				logger.Info(message, args);
			}
		}

		private NLog.Logger GetLogger()
		{
			return DependencyResolver.Current.GetService<NLog.Logger>();
		}

		private string GetEncryptedPassword(string userName, string password)
		{
			return Crypto.HashPassword(userName.ToLower() + password);
		}
		private MembershipUser CreateMembershipUser(SrirachaUser user)
		{
			return new MembershipUser(providerName: SrirachaMembershipProvider.ProviderName,
										name: user.UserName,
										providerUserKey: user.UserGuid,
										email: user.EmailAddress,
										passwordQuestion: user.PasswordQuestion,
										creationDate: user.CreatedDateTimcUtc,
										isLockedOut: user.LockedIndicator,
										comment: null,
										isApproved: true,
										lastActivityDate: user.LastLoginDateDateTimeUtc.GetValueOrDefault(),
										lastLockoutDate: user.LastLockoutDateTimeUtc.GetValueOrDefault(),
										lastLoginDate: user.LastLoginDateDateTimeUtc.GetValueOrDefault(),
										lastPasswordChangedDate: user.LastPasswordChangedDateTimeUtc.GetValueOrDefault());
		}

		private MembershipUserCollection CreateMembershipCollection(PagedSortedList<SrirachaUser> pagedList)
		{
			var returnList = new MembershipUserCollection();
			foreach (var userItem in pagedList.Items)
			{
				var membershipItem = CreateMembershipUser(userItem);
				returnList.Add(membershipItem);
			}
			return returnList;

		}

		public override bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			try 
			{
				var repository = this.GetRepository();
				var user = repository.LoadUserByUserName(userName);
				if(!this.VerifyPassword(user.EncryptedPassword, userName.ToLower(), oldPassword))
				{
					this.LogInfo("Failed to changed password for user {0}, invalid password provided", userName);
					return false;
				}
				else 
				{
					user.EncryptedPassword = GetEncryptedPassword(userName, newPassword);
					user.LastPasswordChangedDateTimeUtc = DateTime.UtcNow;
					repository.UpdateUser(user);
					this.LogInfo("Changed password for user {0}", userName);
					return true;
				}
			}
			catch(Exception err)
			{
				this.LogException(err);
			}
			return false;
		}

		private bool VerifyPassword(string encyptedPassword, string userName, string decryptedPassword)
		{
			return Crypto.VerifyHashedPassword(encyptedPassword, userName.ToLower()+decryptedPassword);
		}


		public override bool ChangePasswordQuestionAndAnswer(string userName, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			try 
			{
				var repository = this.GetRepository();
				var user = repository.LoadUserByUserName(userName);
				if(!this.VerifyPassword(user.EncryptedPassword, userName, password))
				{
					this.LogInfo("Failed to changed password for user {0}, invalid password provided", userName);
					return false;
				}
				else 
				{
					user.PasswordQuestion = newPasswordQuestion;
					user.PasswordAnswer = newPasswordAnswer;
					repository.UpdateUser(user);
					this.LogInfo("Changed password for user {0}", userName);
					return true;
				}
			}
			catch(Exception err)
			{
				this.LogException(err);
			}
			return false;
		}

		public override MembershipUser CreateUser(string userName, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			var repository = this.GetRepository();
			MembershipUser returnValue = null;
			if(string.IsNullOrWhiteSpace(userName))
			{
				status = MembershipCreateStatus.InvalidUserName;
			}
			else if (string.IsNullOrWhiteSpace(password))
			{
				status = MembershipCreateStatus.InvalidPassword;
			}
			else if (string.IsNullOrWhiteSpace(email))
			{
				status = MembershipCreateStatus.InvalidEmail;
			}
			else if (repository.UserNameExists(userName))
			{
				status = MembershipCreateStatus.DuplicateUserName;
			}
			else if (repository.EmailAddressExists(email))
			{
				status = MembershipCreateStatus.DuplicateEmail;
			}
			else 
			{
				var user = new SrirachaUser
				{
					UserName = userName,
					EmailAddress = email,
					PasswordQuestion = passwordQuestion,
					PasswordAnswer = passwordAnswer,
					UserGuid = (Guid)(providerUserKey ?? Guid.NewGuid()),
					LastLockoutDateTimeUtc = null,
					LastLoginDateDateTimeUtc = null,
					LastPasswordChangedDateTimeUtc = DateTime.UtcNow,
					LockedIndicator = false,
					MustChangePasswordIndicator = true,
					EncryptedPassword = GetEncryptedPassword(userName, password)
				};
				user = repository.CreateUser(user);

				returnValue = CreateMembershipUser(user);

				status = MembershipCreateStatus.Success;
				this.LogInfo("Created user {0}", userName);
			}
			return returnValue;			
		}

		public override bool DeleteUser(string userName, bool deleteAllRelatedData)
		{
			var repository = this.GetRepository();
			var user = repository.TryLoadUserByUserName(userName);
			if(user == null)
			{
				return false;
			}
			else 
			{
				repository.DeleteUser(user);
				this.LogInfo("Deleted user {0}", userName);
				return true;
			}
		}

		public override bool EnablePasswordReset
		{
			get { return false; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return false; }
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			var repository = this.GetRepository();
			var listOptions = new ListOptions
			{
				PageNumber = pageIndex,
				PageSize = pageSize,
				SortField="EmailAddress", 
				SortAscending = true
			};
			var pagedList = repository.GetUserList(listOptions, (i)=>i.UserName.Contains(emailToMatch));
			totalRecords = pagedList.TotalItemCount;
			return CreateMembershipCollection(pagedList);
		}

		public override MembershipUserCollection FindUsersByName(string userNameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			var repository = this.GetRepository();
			var listOptions = new ListOptions
			{
				PageNumber = pageIndex+1,
				PageSize = pageSize,
				SortField="UserName", 
				SortAscending = true
			};
			var pagedList = repository.GetUserList(listOptions, (i)=>i.UserName.Contains(userNameToMatch));
			totalRecords = pagedList.TotalItemCount;
			return CreateMembershipCollection(pagedList);
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			var repository = this.GetRepository();
			var listOptions = new ListOptions
			{
				PageNumber = pageIndex+1,
				PageSize = pageSize,
				SortField="UserName", 
				SortAscending = true
			};
			var pagedList = repository.GetUserList(listOptions);
			totalRecords = pagedList.TotalItemCount;
			return CreateMembershipCollection(pagedList);
		}

		public override int GetNumberOfUsersOnline()
		{
			var repository = this.GetRepository();
			DateTime fifteenMinutesAgo = DateTime.UtcNow.AddMinutes(-15);
			return repository.GetUserCount(i=>i.LastActivityDateTimeUtc > fifteenMinutesAgo);
		}

		public override string GetPassword(string userName, string answer)
		{
			throw new ProviderException("GetPassword not supported");
		}

		public override MembershipUser GetUser(string userName, bool userIsOnline)
		{
			var repository = this.GetRepository();
			var user = repository.TryLoadUserByUserName(userName);
			if(user == null)
			{
				return null;
			}
			else 
			{
				if(userIsOnline)
				{
					user.LastActivityDateTimeUtc = DateTime.UtcNow;
					repository.TryUpdateUser(user);
				}
				var membershipUser = CreateMembershipUser(user);
				return membershipUser;
			}
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			Guid userGuid = (Guid)providerUserKey;
			var repository = this.GetRepository();
			var user = repository.TryLoadUserByUserGuid(userGuid);
			if (user == null)
			{
				return null;
			}
			else
			{
				if (userIsOnline)
				{
					user.LastActivityDateTimeUtc = DateTime.UtcNow;
					repository.UpdateUser(user);
				}
				var membershipUser = CreateMembershipUser(user);
				return membershipUser;
			}
		}

		public override string GetUserNameByEmail(string email)
		{
			var repository = this.GetRepository();
			var user = repository.TryLoadUserByEmailAddress(email);
			if(user == null)
			{
				return string.Empty;
			}
			else 
			{
				return user.UserName;
			}
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return 5; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return 0; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return 6; }
		}

		public override int PasswordAttemptWindow
		{
			get { return 5; }
		}
		public override MembershipPasswordFormat PasswordFormat
		{
			get { return MembershipPasswordFormat.Hashed; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return @"^(?=.*\d).{6,}$"; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return false; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return true; }
		}

		public override string ResetPassword(string username, string answer)
		{
			throw new ProviderException("ResetPassword not supported");
		}
		public override bool UnlockUser(string userName)
		{
			var repository = this.GetRepository();
			var user = repository.TryLoadUserByUserName(userName);
			if(user == null)
			{
				return false;
			}
			else 
			{
				if(user.LockedIndicator)
				{
					user.LockedIndicator = false;
					repository.UpdateUser(user);
				}
				this.LogInfo("Unlocked user {0}", userName);
				return true;
			}
		}

		public override void UpdateUser(MembershipUser membershipUser)
		{
			var repository = this.GetRepository();
			var user = repository.LoadUserByUserName(membershipUser.UserName);
			user.EmailAddress = membershipUser.Email;
			repository.UpdateUser(user);
		}

		public override bool ValidateUser(string userName, string password)
		{
			var repository = this.GetRepository();
			var user = repository.TryLoadUserByUserName(userName);
			if(user == null)
			{
				return false;
			}
			else if(!this.VerifyPassword(user.EncryptedPassword, userName, password))
			{
				return false;
			}
			else 
			{
				user.LastActivityDateTimeUtc = DateTime.UtcNow;
				user.LastLoginDateDateTimeUtc = DateTime.UtcNow;
				repository.UpdateUser(user);
				return true;
			}
		}
	}
}