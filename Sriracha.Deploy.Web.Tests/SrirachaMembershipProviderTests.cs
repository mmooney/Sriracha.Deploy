using System;
using System.Configuration.Provider;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;
using System.Web.Security;
using MMDB.Shared;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Web.Security;

namespace Sriracha.Deploy.Web.Tests
{
	public class SrirachaMembershipProviderTests
	{
		private class UserTestData
		{
			public Mock<IMembershipRepository> Repository { get; set; }
			public string UserName { get; set; }
			public string Password { get; set; }
			public string EncryptedPassword 
			{
				get 
				{
					return Crypto.HashPassword(this.UserName.ToLower() + this.Password);
				}
			}
			public string EmailAddress { get; set; }
			public string PasswordQuestion { get; set; }
			public string PasswordAnswer { get; set; }
			public Guid ProviderUserKey { get; set; }
			public bool IsApproved { get; set; }
			public SrirachaUser SrirachaUser { get; set; }

			public static UserTestData Setup(bool userExists)
			{
				var testData = new UserTestData()
				{
					Repository = new Mock<IMembershipRepository>(),
					UserName = Guid.NewGuid().ToString(),
					Password = Guid.NewGuid().ToString(),
					EmailAddress = Guid.NewGuid().ToString() + "@example.com",
					PasswordAnswer = Guid.NewGuid().ToString(),
					PasswordQuestion = Guid.NewGuid().ToString(),
					IsApproved = true,
					ProviderUserKey = Guid.NewGuid()
				};

				if(userExists)
				{
					testData.SrirachaUser = new SrirachaUser()
					{
						UserName = testData.UserName,
						PasswordQuestion = testData.PasswordQuestion,
						PasswordAnswer = testData.PasswordAnswer,
						EmailAddress = testData.EmailAddress,
						UserGuid = (Guid)testData.ProviderUserKey,
						CreatedDateTimcUtc = DateTime.UtcNow.AddDays(-5),
						CreatedByUserName = "UserTestData",
						UpdatedDateTimeUtc = DateTime.UtcNow.AddDays(-10),
						UpdatedByUserName = "UserTestData"
					};
					testData.SrirachaUser.EncryptedPassword = testData.EncryptedPassword;
					testData.Repository.Setup(i => i.GetUserByUserName(It.Is<string>(j => j == testData.UserName))).Returns(testData.SrirachaUser);
					testData.Repository.Setup(i => i.TryGetUserByUserName(It.Is<string>(j => j == testData.UserName))).Returns(testData.SrirachaUser);
					testData.Repository.Setup(i => i.GetUserByUserGuid(It.Is<Guid>(j => j == testData.ProviderUserKey))).Returns(testData.SrirachaUser);
					testData.Repository.Setup(i => i.TryGetUserByUserGuid(It.Is<Guid>(j => j == testData.ProviderUserKey))).Returns(testData.SrirachaUser);
					testData.Repository.Setup(i => i.GetUserByEmailAddress(It.Is<string>(j => j == testData.EmailAddress))).Returns(testData.SrirachaUser);
					testData.Repository.Setup(i => i.TryGetUserByEmailAddress(It.Is<string>(j => j == testData.EmailAddress))).Returns(testData.SrirachaUser);
				}
				else 
				{
					testData.Repository.Setup(i => i.GetUserByUserName(It.IsAny<string>())).Throws(new RecordNotFoundException(typeof(SrirachaUser), "UserName", testData.UserName));
					testData.Repository.Setup(i => i.TryGetUserByUserName(It.IsAny<string>())).Returns((SrirachaUser)null);
					testData.Repository.Setup(i => i.GetUserByUserGuid(It.IsAny<Guid>())).Throws(new RecordNotFoundException(typeof(SrirachaUser), "UserGuid", (Guid)testData.ProviderUserKey));
					testData.Repository.Setup(i => i.TryGetUserByUserGuid(It.IsAny<Guid>())).Returns((SrirachaUser)null);
					testData.Repository.Setup(i => i.GetUserByEmailAddress(It.IsAny<string>())).Throws(new RecordNotFoundException(typeof(SrirachaUser), "EmailAddress", testData.EmailAddress));
					testData.Repository.Setup(i => i.TryGetUserByEmailAddress(It.IsAny<string>())).Returns((SrirachaUser)null);
				}
				testData.Repository.Setup(i=>i.CreateUser(It.IsAny<SrirachaUser>())).Returns((SrirachaUser inputUser)=>{inputUser.Id=inputUser.UserName; return inputUser;});
				return testData;
			}
		}

		public class ApplicationName
		{
			[Test]
			public void DefaultsToSriracha()
			{
				var provider = new SrirachaMembershipProvider();
				Assert.AreEqual("Sriracha", provider.ApplicationName);
			}

			[Test]
			public void IsUpdatable()
			{
				SrirachaMembershipProvider provider = new SrirachaMembershipProvider();
				string testValue = Guid.NewGuid().ToString();
				provider.ApplicationName = testValue;
				Assert.AreEqual(testValue, provider.ApplicationName);
			}
		}

		public class ChangePassword
		{
			[Test]
			public void ShouldWriteHashedValue()
			{
				var testData = UserTestData.Setup(true);
				string newPassword = Guid.NewGuid().ToString();

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePassword(testData.UserName, testData.Password, newPassword);
				Assert.IsTrue(result);
				Assert.IsTrue(Crypto.VerifyHashedPassword(testData.SrirachaUser.EncryptedPassword, testData.SrirachaUser.UserName.ToLower()+newPassword));
				testData.Repository.Verify(i => i.UpdateUser(testData.SrirachaUser), Times.Once());
			}

			[Test]
			public void ShouldUpdatePasswordChangeDate()
			{
				var testData = UserTestData.Setup(true);
				string newPassword = Guid.NewGuid().ToString();
				DateTime oldPasswordChangedDate = DateTime.UtcNow.AddDays(-1);
				testData.SrirachaUser.LastPasswordChangedDateTimeUtc = oldPasswordChangedDate;

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePassword(testData.UserName, testData.Password, newPassword);
				Assert.IsTrue(result);
				Assert.Greater(testData.SrirachaUser.LastPasswordChangedDateTimeUtc, oldPasswordChangedDate);
				testData.Repository.Verify(i => i.UpdateUser(testData.SrirachaUser), Times.Once());
			}

			[Test]
			public void OldPasswordIsRequired()
			{
				var testData = UserTestData.Setup(true);
				string oldEncryptedPassword = testData.SrirachaUser.EncryptedPassword;
				string wrongOldPassword = "SomeIncorrectOtherValue";
				string newPassword = Guid.NewGuid().ToString();

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePassword(testData.UserName, wrongOldPassword, newPassword);
				Assert.IsFalse(result);
				Assert.AreEqual(oldEncryptedPassword, testData.SrirachaUser.EncryptedPassword);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void ValidUserNameRequired()
			{
				var testData = UserTestData.Setup(false);
				string newPassword = Guid.NewGuid().ToString();

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePassword(testData.UserName, testData.Password, newPassword);
				Assert.IsFalse(result);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}
		}

		public class ChangePasswordQuestionAndAnswer
		{
			[Test]
			public void ShouldUpdatePasswordQuestionAndAnswer()
			{
				var testData = UserTestData.Setup(true);
				string newPasswordQuestion = Guid.NewGuid().ToString();
				string newPasswordAnswer = Guid.NewGuid().ToString();
				
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePasswordQuestionAndAnswer(testData.UserName, testData.Password, newPasswordQuestion, newPasswordAnswer);
				Assert.IsTrue(result);
				Assert.AreEqual(newPasswordQuestion, testData.SrirachaUser.PasswordQuestion);
				Assert.AreEqual(newPasswordAnswer, testData.SrirachaUser.PasswordAnswer);
				testData.Repository.Verify(i => i.UpdateUser(testData.SrirachaUser), Times.Once());
			}

			[Test]
			public void RequiresValidUserName()
			{
				var testData = UserTestData.Setup(false);
				string newPasswordQuestion = Guid.NewGuid().ToString();
				string newPasswordAnswer = Guid.NewGuid().ToString();

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePasswordQuestionAndAnswer(testData.UserName, testData.Password, newPasswordQuestion, newPasswordAnswer);
				Assert.IsFalse(result);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void RequiresValidPassword()
			{
				var testData = UserTestData.Setup(true);
				string wrongPassword = "SomeOtherValue";
				string newPasswordQuestion = Guid.NewGuid().ToString();
				string newPasswordAnswer = Guid.NewGuid().ToString();

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ChangePasswordQuestionAndAnswer(testData.UserName, wrongPassword, newPasswordQuestion, newPasswordAnswer);
				Assert.IsFalse(result);
				Assert.AreEqual(testData.PasswordQuestion, testData.SrirachaUser.PasswordQuestion);
				Assert.AreEqual(testData.PasswordAnswer, testData.SrirachaUser.PasswordAnswer);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}
		}

		public class CreateUser
		{
			[Test]
			public void ShouldCreateUser()
			{
				var testData = UserTestData.Setup(false);

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNotNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.Success, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Once());
			}

			[Test]
			public void UserNameIsRequired()
			{
				var testData = UserTestData.Setup(false);
				testData.UserName = null;

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.InvalidUserName, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void PasswordIsRequrred()
			{
				var testData = UserTestData.Setup(false);
				testData.Password = null;

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.InvalidPassword, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void UserNameMustBeUnique()
			{
				var testData = UserTestData.Setup(false);
				testData.Repository.Setup(i=>i.UserNameExists(It.Is<string>(j=>j==testData.UserName))).Returns(true);

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.DuplicateUserName, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void EmailAddressIsRequired()
			{
				var testData = UserTestData.Setup(false);
				testData.EmailAddress = null;

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);
				
				Assert.IsNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.InvalidEmail, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void EmailAddressMustBeUnique()
			{
				var testData = UserTestData.Setup(false);
				testData.Repository.Setup(i => i.EmailAddressExists(It.Is<string>(j => j == testData.EmailAddress))).Returns(true);

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.DuplicateEmail, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}

			[Test]
			public void PasswordQuestionIsNotRequired()
			{
				var testData = UserTestData.Setup(false);
				testData.PasswordQuestion = null;

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNotNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.Success, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Once());
			}

			[Test]
			public void PasswordAnswerIsNotRequired()
			{
				var testData = UserTestData.Setup(false);
				testData.PasswordAnswer = null;

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNotNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.Success, status);
				testData.Repository.Verify(i => i.CreateUser(It.IsAny<SrirachaUser>()), Times.Once());
			}

			[Test]
			public void MustChangePasswordDefaultsToTrue()
			{
				var testData = UserTestData.Setup(false);

				MembershipCreateStatus status;
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.CreateUser(testData.UserName, testData.Password, testData.EmailAddress, testData.PasswordQuestion, testData.PasswordAnswer, testData.IsApproved, testData.ProviderUserKey, out status);

				Assert.IsNotNull(membershipUser);
				Assert.AreEqual(MembershipCreateStatus.Success, status);
				testData.Repository.Verify(i => i.CreateUser(It.Is<SrirachaUser>(j=>j.MustChangePasswordIndicator == true)), Times.Once());
			}
		}

		public class DeleteUser
		{
			[Test]
			public void ShouldDeleteUser()
			{
				var testData = UserTestData.Setup(true);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.DeleteUser(testData.UserName, true);

				Assert.IsTrue(result);
				testData.Repository.Verify(i=>i.DeleteUser(It.Is<SrirachaUser>(j=>j.UserName == testData.UserName)), Times.Once());
			}

			[Test]
			public void UserNameMustBeValid()
			{
				var testData = UserTestData.Setup(false);
				string wrongUserName = Guid.NewGuid().ToString();

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.DeleteUser(wrongUserName, true);

				Assert.IsFalse(result);
				testData.Repository.Verify(i => i.DeleteUser(It.Is<SrirachaUser>(j => j.UserName == testData.UserName)), Times.Never());
			}
		}

		public class EnablePasswordReset
		{
			[Test]
			public void ShouldBeFalse()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.IsFalse(provider.EnablePasswordReset);
			}
		}

		public class EnablePasswordRetrieval
		{
			[Test]
			public void ShouldBeFalse()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.IsFalse(provider.EnablePasswordRetrieval);
			}
		}

		public class FindUsersByEmail
		{
			[Test]
			public void ShouldReturnUsersByEmailMatch()
			{
				var testData = UserTestData.Setup(false);
				var userList = new SrirachaUser[] 
				{
					new SrirachaUser { UserName="test1", EmailAddress = "test1@example.com" },
					new SrirachaUser { UserName="test2", EmailAddress = "test2@example.com" },
					new SrirachaUser { UserName="test3", EmailAddress = "test3@example.com" },
					new SrirachaUser { UserName="test4", EmailAddress = "test4@example.com" },
					new SrirachaUser { UserName="test5", EmailAddress = "test5@example.com" },
				};
				string emailToMatch = "test1@example.com";
				var matchedList = userList.Where(i=>i.EmailAddress.Contains(emailToMatch));
				var pagedList = new PagedSortedList<SrirachaUser>(new PagedList.StaticPagedList<SrirachaUser>(matchedList,  1, int.MaxValue, matchedList.Count()), string.Empty, true);
                testData.Repository.Setup(i => i.GetUserList_old(It.IsAny<ListOptions>(), It.IsAny<Expression<Func<SrirachaUser, bool>>>())).Returns(pagedList);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				int totalRecords;
				var collection = provider.FindUsersByEmail(emailToMatch, 0, 10, out totalRecords);

				Assert.AreEqual(1, totalRecords);
				Assert.IsNotNull(collection);
				Assert.AreEqual(1, collection.Count);
				Assert.IsNotNull(collection["test1"]);
			}
		}

		public class FindUsersByName
		{
			[Test]
			public void ShouldReturnUsersByUserNameMatch()
			{
				var testData = UserTestData.Setup(false);
				var userList = new SrirachaUser[] 
				{
					new SrirachaUser { UserName="test1", EmailAddress = "test1@example.com" },
					new SrirachaUser { UserName="test2", EmailAddress = "test2@example.com" },
					new SrirachaUser { UserName="test3", EmailAddress = "test3@example.com" },
					new SrirachaUser { UserName="test4", EmailAddress = "test4@example.com" },
					new SrirachaUser { UserName="test5", EmailAddress = "test5@example.com" },
				};
				string userNameToMatch = "test1";
				var matchedList = userList.Where(i=>i.EmailAddress.Contains(userNameToMatch));
				var pagedList = new PagedSortedList<SrirachaUser>(new PagedList.StaticPagedList<SrirachaUser>(matchedList,  1, int.MaxValue, matchedList.Count()), string.Empty, true);
                testData.Repository.Setup(i => i.GetUserList_old(It.IsAny<ListOptions>(), It.IsAny<Expression<Func<SrirachaUser, bool>>>())).Returns(pagedList);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				int totalRecords;
				var collection = provider.FindUsersByName(userNameToMatch, 0, 10, out totalRecords);

				Assert.AreEqual(1, totalRecords);
				Assert.IsNotNull(collection);
				Assert.AreEqual(1, collection.Count);
				Assert.IsNotNull(collection["test1"]);
			}
		}

		public class GetAllUsers
		{
			[Test]
			public void ShouldReturnUserList()
			{
				var testData = UserTestData.Setup(false);
				var userList = new SrirachaUser[] 
				{
					new SrirachaUser { UserName="test1", EmailAddress = "test1@example.com" },
					new SrirachaUser { UserName="test2", EmailAddress = "test2@example.com" },
					new SrirachaUser { UserName="test3", EmailAddress = "test3@example.com" },
					new SrirachaUser { UserName="test4", EmailAddress = "test4@example.com" },
					new SrirachaUser { UserName="test5", EmailAddress = "test5@example.com" },
				};
				var pagedList = new PagedSortedList<SrirachaUser>(new PagedList.StaticPagedList<SrirachaUser>(userList,  1, int.MaxValue, userList.Count()), string.Empty, true);
                testData.Repository.Setup(i => i.GetUserList(It.Is<ListOptions>(j => j.PageNumber == 1 && j.PageSize == 10), null)).Returns(pagedList);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				int totalRecords;
				var collection = provider.GetAllUsers(0, 10, out totalRecords);

				Assert.AreEqual(5, totalRecords);
				Assert.IsNotNull(collection);
				Assert.AreEqual(5, collection.Count);
				Assert.IsNotNull(collection["test1"]);
				Assert.IsNotNull(collection["test2"]);
				Assert.IsNotNull(collection["test3"]);
				Assert.IsNotNull(collection["test4"]);
				Assert.IsNotNull(collection["test5"]);
			}

		}

		public class GetNumberOfUsersOnline
		{
			[Test]
			public void ShouldReturnListOfUsersWhoseLastActivityDateWas15MinutesAgo()
			{
				var testData = UserTestData.Setup(false);
				testData.Repository.Setup(i => i.GetUserCount(It.IsAny<Expression<Func<SrirachaUser, bool>>>())).Returns(10);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				int userCount = provider.GetNumberOfUsersOnline();

				Assert.AreEqual(10, userCount);
			}
		}

		public class GetPassword
		{
			[Test]
			public void ShouldThrowException()
			{
				var testData = UserTestData.Setup(false);
				
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.Throws(typeof(ProviderException), delegate { provider.GetPassword(null, null); });
			}
		}

		public class GetUser
		{
			[Test]
			public void ShouldReturnUserByUserName()
			{
				var testData = UserTestData.Setup(true);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(testData.SrirachaUser.UserName, false);
				
				Assert.IsNotNull(membershipUser);
				Assert.AreEqual(testData.SrirachaUser.UserName, membershipUser.UserName);
				Assert.AreEqual(testData.SrirachaUser.UserGuid, membershipUser.ProviderUserKey);
			}

			[Test]
			public void ShouldReturnNullForInvalidUserName()
			{
				var testData = UserTestData.Setup(true);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(Guid.NewGuid().ToString(), false);

				Assert.IsNull(membershipUser);
			}

			[Test]
			public void ShouldUpdateLastActivityDateForUserNameIfOnline()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LastActivityDateTimeUtc = DateTime.UtcNow.AddMonths(-1);
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(testData.SrirachaUser.UserName, true);

				Assert.IsNotNull(membershipUser);
				testData.Repository.Verify(i => i.TryUpdateUser(testData.SrirachaUser), Times.Once());
				Assert.Greater(testData.SrirachaUser.LastActivityDateTimeUtc, DateTime.UtcNow.AddMinutes(-1));
			}

			[Test]
			public void ShouldNotUpdateLastActivityDateForUserNameIfNotOnline()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LastActivityDateTimeUtc = DateTime.UtcNow.AddMonths(-1);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(testData.SrirachaUser.UserName, false);

				Assert.IsNotNull(membershipUser);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
				Assert.Less(testData.SrirachaUser.LastActivityDateTimeUtc, DateTime.UtcNow.AddMinutes(-1));
			}


			[Test]
			public void ShouldReturnUserByProviderKey()
			{
				var testData = UserTestData.Setup(true);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(testData.SrirachaUser.UserGuid, false);

				Assert.IsNotNull(membershipUser);
				Assert.AreEqual(testData.SrirachaUser.UserName, membershipUser.UserName);
				Assert.AreEqual(testData.SrirachaUser.UserGuid, membershipUser.ProviderUserKey);
			}

			[Test]
			public void ShouldReturnNullForInvalidProviderKey()
			{
				var testData = UserTestData.Setup(true);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(Guid.NewGuid(), false);

				Assert.IsNull(membershipUser);
			}

			[Test]
			public void ShouldUpdateLastActivityDateForProviderKeyIfOnline()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LastActivityDateTimeUtc = DateTime.UtcNow.AddMonths(-1);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(testData.SrirachaUser.UserGuid, true);

				Assert.IsNotNull(membershipUser);
				testData.Repository.Verify(i => i.UpdateUser(testData.SrirachaUser), Times.Once());
				Assert.Greater(testData.SrirachaUser.LastActivityDateTimeUtc, DateTime.UtcNow.AddMinutes(-1));
			}

			[Test]
			public void ShouldNotUpdateLastActivityDateForProviderKeyIfNotOnline()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LastActivityDateTimeUtc = DateTime.UtcNow.AddMonths(-1);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				var membershipUser = provider.GetUser(testData.SrirachaUser.UserGuid, false);

				Assert.IsNotNull(membershipUser);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}
		}

		public class GetUserNameByEmail
		{
			[Test]
			public void ShouldReturnUserNameByEmail()
			{
				var testData = UserTestData.Setup(true);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				string userName = provider.GetUserNameByEmail(testData.SrirachaUser.EmailAddress);
				
				Assert.AreEqual(testData.SrirachaUser.UserName, userName);
			}

			[Test]
			public void ShouldReturnEmptyStringForInvalidEmail()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				string userName = provider.GetUserNameByEmail(Guid.NewGuid().ToString());

				Assert.AreEqual(string.Empty, userName);
			}
		}

		public class MaxInvalidPasswordAttempts
		{
			[Test]
			public void ShouldBeFive()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.AreEqual(5, provider.MaxInvalidPasswordAttempts);
			}
		}

		public class MinRequiredNonAlphanumericCharacters
		{
			[Test]
			public void ShouldBeZero()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.AreEqual(0, provider.MinRequiredNonAlphanumericCharacters);
			}
		}

		public class MinRequiredPasswordLength
		{
			[Test]
			public void ShouldBeSix()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.AreEqual(6, provider.MinRequiredPasswordLength);
			}
		}

		public class PasswordAttemptWindow
		{
			[Test]
			public void ShouldBeFiveMinutes()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.AreEqual(5, provider.PasswordAttemptWindow);
			}
		}

		public class PasswordFormat
		{
			[Test]
			public void ShouldBeHashed()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.AreEqual(MembershipPasswordFormat.Hashed, provider.PasswordFormat);
			}
		}

		public class PasswordStrengthRegularExpression
		{
			[Test]
			public void ShouldMatchSecurityConstantsValue()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.AreEqual(@"^(?=.*\d).{6,}$", provider.PasswordStrengthRegularExpression);
			}
		}

		public class RequiresQuestionAndAnswer
		{
			[Test]
			public void ShouldBeFalse()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.IsFalse(provider.RequiresQuestionAndAnswer);
			}
		}

		public class RequiresUniqueEmail
		{
			[Test]
			public void ShouldBeTrue()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.IsTrue(provider.RequiresUniqueEmail);
			}
		}

		public class ResetPassword
		{
			[Test]
			public void ShouldThrowException()
			{
				var testData = UserTestData.Setup(false);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.Throws(typeof(ProviderException), delegate { provider.ResetPassword(null, null); });
			}
		}

		public class UnlockUser
		{
			[Test]
			public void ShouldResetIsLockedFlag()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LockedIndicator = true;

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.UnlockUser(testData.SrirachaUser.UserName);

				Assert.IsTrue(result);
				Assert.IsFalse(testData.SrirachaUser.LockedIndicator);
				testData.Repository.Verify(i=>i.UpdateUser(testData.SrirachaUser), Times.Once());
			}

			[Test]
			public void ShouldSucceedIfUserAlreadyLocked()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LockedIndicator = false;

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.UnlockUser(testData.SrirachaUser.UserName);

				Assert.IsTrue(result);
				Assert.IsFalse(testData.SrirachaUser.LockedIndicator);
			}

			[Test]
			public void ShouldFailIfUserNameInvalid()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LockedIndicator = true;

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.UnlockUser(Guid.NewGuid().ToString());

				Assert.IsFalse(result);
				Assert.IsTrue(testData.SrirachaUser.LockedIndicator);
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}
		}

		public class UpdateUser
		{
			[Test]
			public void ShouldUpdateUserFields()
			{
				var testData = UserTestData.Setup(true);
				var membershipUser = new MembershipUser
				(
					providerName: SrirachaMembershipProvider.ProviderName,
					name: testData.SrirachaUser.UserName,
					providerUserKey: testData.SrirachaUser.UserGuid,
					email: Guid.NewGuid().ToString() + "@example.com",
					passwordQuestion: Guid.NewGuid().ToString(),
					creationDate: testData.SrirachaUser.CreatedDateTimcUtc,
					isLockedOut: testData.SrirachaUser.LockedIndicator,
					comment: null,
					isApproved: true,
					lastActivityDate: DateTime.UtcNow,
					lastLockoutDate: DateTime.MinValue,
					lastLoginDate: DateTime.UtcNow,
					lastPasswordChangedDate: DateTime.MinValue
				);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				provider.UpdateUser(membershipUser);

				Assert.AreEqual(membershipUser.Email, testData.SrirachaUser.EmailAddress);
				testData.Repository.Verify(i=>i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Once());
			}

			[Test]
			public void ShouldThrowExceptionForInvalidUserName()
			{
				var testData = UserTestData.Setup(false);
				var membershipUser = new MembershipUser
				(
					providerName: SrirachaMembershipProvider.ProviderName,
					name: Guid.NewGuid().ToString(),
					providerUserKey: Guid.NewGuid(),
					email: Guid.NewGuid().ToString() + "@example.com",
					passwordQuestion: Guid.NewGuid().ToString(),
					creationDate: DateTime.MinValue,
					isLockedOut: false,
					comment: null,
					isApproved: true,
					lastActivityDate: DateTime.UtcNow,
					lastLockoutDate: DateTime.MinValue,
					lastLoginDate: DateTime.UtcNow,
					lastPasswordChangedDate: DateTime.MinValue
				);

				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				Assert.Throws(typeof(RecordNotFoundException), delegate { provider.UpdateUser(membershipUser); });
				testData.Repository.Verify(i => i.UpdateUser(It.IsAny<SrirachaUser>()), Times.Never());
			}
		}

		public class ValidateUser
		{
			[Test]
			public void ShouldAcceptValidUserNamePassword()
			{
				var testData = UserTestData.Setup(true);
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ValidateUser(testData.UserName, testData.Password);

				Assert.IsTrue(result);
			}

			[Test]
			public void ShouldUpdateLastLogicDateUponSuccess()
			{
				var testData = UserTestData.Setup(true);
				testData.SrirachaUser.LastLoginDateDateTimeUtc = DateTime.UtcNow.AddDays(-1);
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ValidateUser(testData.UserName, testData.Password);

				Assert.IsTrue(result);
				Assert.Less(DateTime.UtcNow.AddMinutes(-1), testData.SrirachaUser.LastLoginDateDateTimeUtc);
				testData.Repository.Verify(i=>i.UpdateUser(testData.SrirachaUser), Times.Once());
			}


			[Test]
			public void ShouldReturnFalseInvalidUserName()
			{
				var testData = UserTestData.Setup(true);
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ValidateUser(testData.UserName, Guid.NewGuid().ToString());

				Assert.IsFalse(result);
			}

			[Test]
			public void ShouldReturnFalseForInvalidUserName()
			{
				var testData = UserTestData.Setup(false);
				var provider = new SrirachaMembershipProvider(testData.Repository.Object);
				bool result = provider.ValidateUser(testData.UserName, testData.Password);

				Assert.IsFalse(result);
			}
		}
	}
}
