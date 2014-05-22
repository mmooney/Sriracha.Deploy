using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Account;
using MMDB.Shared;
using Sriracha.Deploy.Data.Exceptions;

namespace Sriracha.Deploy.Data.Tests.Repository.Account
{
    public abstract class MembershipRepositoryTests : RepositoryTestBase<IMembershipRepository>
    {
        //private string FormatId(string userName)
        //{
        //    return "SrirachaUser_" + userName.Replace('\\', '_');
        //}
        
        private void AssertCreatedUser(SrirachaUser expected, SrirachaUser actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
                Assert.AreNotEqual(Guid.Empty, actual.UserGuid);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
                Assert.AreEqual(expected.EncryptedPassword, actual.EncryptedPassword);
                AssertDateEqual(expected.LastPasswordChangedDateTimeUtc, actual.LastPasswordChangedDateTimeUtc);
                Assert.AreEqual(expected.PasswordQuestion, actual.PasswordQuestion);
                Assert.AreEqual(expected.PasswordAnswer, actual.PasswordAnswer);
                AssertDateEqual(expected.LastLockoutDateTimeUtc, actual.LastLockoutDateTimeUtc);
                AssertDateEqual(expected.LastLoginDateDateTimeUtc, actual.LastLoginDateDateTimeUtc);
                Assert.AreEqual(expected.LockedIndicator, actual.LockedIndicator);
                Assert.AreEqual(expected.MustChangePasswordIndicator, actual.MustChangePasswordIndicator);
                AssertDateEqual(expected.LastActivityDateTimeUtc, actual.LastActivityDateTimeUtc);

                AssertProjectNotififcationList(expected.ProjectNotificationItemList, actual.ProjectNotificationItemList);
            }
        }

        private void AssertUpdatedUser(SrirachaUser original, SrirachaUser expected, SrirachaUser actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                AssertHelpers.AssertUpdatedBaseDto(original, actual, this.UserName);
                Assert.AreEqual(expected.UserGuid, actual.UserGuid);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
                Assert.AreEqual(expected.EncryptedPassword, actual.EncryptedPassword);
                AssertDateEqual(expected.LastPasswordChangedDateTimeUtc, actual.LastPasswordChangedDateTimeUtc);
                Assert.AreEqual(expected.PasswordQuestion, actual.PasswordQuestion);
                Assert.AreEqual(expected.PasswordAnswer, actual.PasswordAnswer);
                AssertDateEqual(expected.LastLockoutDateTimeUtc, actual.LastLockoutDateTimeUtc);
                AssertDateEqual(expected.LastLoginDateDateTimeUtc, actual.LastLoginDateDateTimeUtc);
                Assert.AreEqual(expected.LockedIndicator, actual.LockedIndicator);
                Assert.AreEqual(expected.MustChangePasswordIndicator, actual.MustChangePasswordIndicator);
                AssertDateEqual(expected.LastActivityDateTimeUtc, actual.LastActivityDateTimeUtc);

                AssertProjectNotififcationList(expected.ProjectNotificationItemList, actual.ProjectNotificationItemList);
            }
        }

        private void AssertUser(SrirachaUser expected, SrirachaUser actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                AssertHelpers.AssertBaseDto(expected, actual);
                Assert.AreEqual(expected.UserGuid, actual.UserGuid);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
                Assert.AreEqual(expected.EncryptedPassword, actual.EncryptedPassword);
                AssertDateEqual(expected.LastPasswordChangedDateTimeUtc, actual.LastPasswordChangedDateTimeUtc);
                Assert.AreEqual(expected.PasswordQuestion, actual.PasswordQuestion);
                Assert.AreEqual(expected.PasswordAnswer, actual.PasswordAnswer);
                AssertDateEqual(expected.LastLockoutDateTimeUtc, actual.LastLockoutDateTimeUtc);
                AssertDateEqual(expected.LastLoginDateDateTimeUtc, actual.LastLoginDateDateTimeUtc);
                Assert.AreEqual(expected.LockedIndicator, actual.LockedIndicator);
                Assert.AreEqual(expected.MustChangePasswordIndicator, actual.MustChangePasswordIndicator);
                AssertDateEqual(expected.LastActivityDateTimeUtc, actual.LastActivityDateTimeUtc);

                AssertProjectNotififcationList(expected.ProjectNotificationItemList, actual.ProjectNotificationItemList);
            }
        }

        private void AssertProjectNotififcationList(List<ProjectNotificationItem> expectedList, List<ProjectNotificationItem> actualList)
        {
            if(expectedList == null)
            {
                Assert.IsNull(actualList);
            }
            else 
            {
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedList.Count, actualList.Count);

                foreach(var expectedItem in expectedList)
                {
                    var actualItem = actualList.FirstOrDefault(i=>i.ProjectId == expectedItem.ProjectId);
                    Assert.IsNotNull(actualItem);
                    Assert.AreEqual(expectedItem.ProjectInactive, actualItem.ProjectInactive);
                    Assert.AreEqual(expectedItem.ProjectName, actualItem.ProjectName);
                    Assert.AreEqual(expectedItem.UserName, actualItem.UserName);
                    Assert.AreEqual(expectedItem.Flags, actualItem.Flags);
                }
            }
        }

        [Test]
        public void CreateUser_CreatesUser()
        {
            var sut = this.GetRepository();
            var user = this.Fixture.Create<SrirachaUser>();

            var result = sut.CreateUser(user);

            Assert.IsNotNull(result);
            AssertCreatedUser(user, result);
            var dbItem = sut.GetUser(result.Id);
            AssertUser(result, dbItem);
        }

        [Test]
        public void CreateUser_NullUser_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.CreateUser(null));
        }


        [Test]
        public void CreateUser_MissingUserName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var user = this.Fixture.Create<SrirachaUser>();
            user.UserName = null;

            Assert.Throws<ArgumentNullException>(() => sut.CreateUser(user));
        }

        [Test]
        public void CreateUser_MissingUserGuid_CreatesUserWithNewGuid()
        {
            var sut = this.GetRepository();
            var user = this.Fixture.Create<SrirachaUser>();
            user.UserGuid = Guid.Empty;

            var result = sut.CreateUser(user);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.UserGuid);
        }

        [Test]
        public void CreateUser_DuplicateUserName_ThrowsArgumentException()
        {
            var sut = this.GetRepository();
            var user = this.Fixture.Create<SrirachaUser>();
            var firstUser = sut.CreateUser(user);

            Assert.Throws<ArgumentException>(()=>sut.CreateUser(user));
        }

        [Test]
        public void UpdateUser_UpdatesUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);
            var updatedData = this.Fixture.Create<SrirachaUser>();
            this.CreateNewUserName();
            updatedData.Id = user.Id;
            updatedData.UserName = user.UserName;

            var result = sut.UpdateUser(updatedData);
            AssertUpdatedUser(user, updatedData, result);
            var dbItem = sut.GetUser(result.Id);
            AssertUser(result, dbItem);
        }

        [Test]
        public void UpdateUser_NullUser_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.UpdateUser(null));
        }

        [Test]
        public void UpdateUser_MissingUserName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            data.UserName = null;

            Assert.Throws<ArgumentNullException>(() => sut.UpdateUser(data));
        }

        [Test]
        public void DeleteUser_DeletesUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.DeleteUser(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.Throws<RecordNotFoundException>(()=>sut.GetUser(user.Id));
        }

        [Test]
        public void DeleteUser_MissingUserID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            Assert.Throws<ArgumentNullException>(()=>sut.DeleteUser(null));
        }


        [Test]
        public void DeleteUser_BadUserID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteUser(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetUser_GetsUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.GetUser(user.Id);

            AssertUser(user, result);
        }

        [Test]
        public void GetUser_MissingUserID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetUser(null));
        }

        [Test]
        public void GetUser_BadUserID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetUser(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetUserByUserName_GetsUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.GetUserByUserName(user.UserName);

            AssertUser(user, result);
        }

        [Test]
        public void GetUserByUserName_MissingUserName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            Assert.Throws<ArgumentNullException>(()=>sut.GetUserByUserName(null));
        }

        [Test]
        public void GetUserByUserName_BadUserName_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            Assert.Throws<RecordNotFoundException>(() => sut.GetUserByUserName(Guid.NewGuid().ToString()));
        }

        [Test]
        public void TryGetUserByUserName_GetsUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.TryGetUserByUserName(user.UserName);

            AssertUser(user, result);
        }

        [Test]
        public void TryGetUserByUserName_MissingUserName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.TryGetUserByUserName(null));
        }

        [Test]
        public void TryGetUserByUserName_BadUserName_ReturnsNull()
        {
            var sut = this.GetRepository();

            var result = sut.TryGetUserByUserName(Guid.NewGuid().ToString());

            Assert.IsNull(result);
        }

        [Test]
        public void TryGetUserByUserGuid_GetsUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.TryGetUserByUserGuid(user.UserGuid);

            AssertUser(user, result);
        }

        [Test]
        public void TryGetUserByUserGuid_MissingUserGuid_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.TryGetUserByUserGuid(Guid.Empty));
        }

        [Test]
        public void TryGetUserByUserGuid_BadUserGuid_ReturnsNull()
        {
            var sut = this.GetRepository();

            var result = sut.TryGetUserByUserGuid(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public void TryGetUserByEmailAddress_GetsUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.TryGetUserByEmailAddress(user.EmailAddress);

            AssertUser(user, result);
        }

        [Test]
        public void TryGetUserByEmailAddress_MissingEmailAddress_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.TryGetUserByEmailAddress(null));
        }

        [Test]
        public void TryGetUserByEmailAddress_BadEmailAddress_ReturnsNull()
        {
            var sut = this.GetRepository();

            var result = sut.TryGetUserByEmailAddress(Guid.NewGuid().ToString());

            Assert.IsNull(result);
        }

        [Test]
        public void UserNameExists_UserExists_ReturnsTrue()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.UserNameExists(user.UserName);

            Assert.IsTrue(result);
        }

        [Test]
        public void UserNameExists_UserDoesNotExist_ReturnsFalse()
        {
            var sut = this.GetRepository();

            var result = sut.UserNameExists(Guid.NewGuid().ToString());

            Assert.IsFalse(result);
        }

        [Test]
        public void UserNameExists_MissingUserName_ThrowNewArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.UserNameExists(null));
        }

        [Test]
        public void EmailAddressExists_UserExists_ReturnsTrue()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaUser>();
            var user = sut.CreateUser(data);

            var result = sut.EmailAddressExists(user.EmailAddress);

            Assert.IsTrue(result);
        }

        [Test]
        public void EmailAddressExists_UserDoesNotExist_ReturnsFalse()
        {
            var sut = this.GetRepository();

            var result = sut.EmailAddressExists(Guid.NewGuid().ToString());

            Assert.IsFalse(result);
        }

        [Test]
        public void EmailAddressExists_MissingEmailAddress_ThrowNewArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.EmailAddressExists(null));
        }

        [Test]
        public void GetUserList_GetsUserList()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 10; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreNotEqual(0, result.Items.Count);
            Assert.AreNotEqual(0, result.PageSize);
        }

        [Test]
        public void GetUserList_Defaults()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(null);

            int defaultPageSize = 20;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(defaultPageSize, result.Items.Count);
            Assert.AreEqual(defaultPageSize, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(2, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsTrue(result.SortAscending);
            Assert.AreEqual("UserName", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetUserList_PageSize()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(new ListOptions { PageSize = 5 });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(5, result.Items.Count);
            Assert.AreEqual(5, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(6, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsTrue(result.SortAscending);
            Assert.AreEqual("UserName", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetUserList_SortByUserNameDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(new ListOptions { SortField = "UserName", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            SrirachaUser lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.UserName, item.UserName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetUserList_SortByUpdatedDateTimeUtcAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(new ListOptions { SortField = "UserName", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            SrirachaUser lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.UserName, item.UserName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetUserList_SortByEmailAddressAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(new ListOptions { SortField = "EmailAddress", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            SrirachaUser lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.EmailAddress, item.EmailAddress);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetUserList_SortByEmailAddressDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserList(new ListOptions { SortField = "EmailAddress", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            SrirachaUser     lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.EmailAddress, item.EmailAddress);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetUserList_BadSortField_ThrowsUnrecognizedSortFieldException()
        {
            var sut = this.GetRepository();

            Assert.Throws<UnrecognizedSortFieldException<SrirachaUser>>(()=>sut.GetUserList(new ListOptions { SortField=Guid.NewGuid().ToString() }));
        }

        [Test]
        public void GetUserList_FilterByUserName()
        {
            var sut = this.GetRepository();

            var userNameList = new List<string>();
            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                var user = sut.CreateUser(data);
                if(i%5 == 0)
                {
                    userNameList.Add(user.UserName);
                }
            }


            var result = sut.GetUserList(null, userNameList);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(userNameList.Count, result.Items.Count);
            Assert.AreEqual(userNameList.Count, result.TotalItemCount);
            foreach(var userName in userNameList)
            {
                Assert.IsTrue(result.Items.Any(i=>i.UserName == userName));
            }
        }

        [Test]
        public void GetUserList_FilterByEmailAddress()
        {
            var sut = this.GetRepository();

            var emailAddressList = new List<string>();
            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                var user = sut.CreateUser(data);
                if (i % 5 == 0)
                {
                    emailAddressList.Add(user.EmailAddress);
                }
            }


            var result = sut.GetUserList(null, null, emailAddressList);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(emailAddressList.Count, result.Items.Count);
            Assert.AreEqual(emailAddressList.Count, result.TotalItemCount);
            foreach (var emailAddress in emailAddressList)
            {
                Assert.IsTrue(result.Items.Any(i => i.EmailAddress == emailAddress));
            }
        }

        [Test]
        public void GetUserCount_GetsUserCount()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserCount();

            Assert.LessOrEqual(30, result);
        }

        [Test]
        public void GetUserCount_UsesLastActivityDate()
        {
            var sut = this.GetRepository();

            DateTime cutOff = DateTime.UtcNow;
            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                if(i%2==1)
                {
                    data.LastActivityDateTimeUtc = cutOff.AddMinutes(-60);
                }
                else 
                {
                    data.LastActivityDateTimeUtc = cutOff.AddMinutes(+60);
                }
                sut.CreateUser(data);
            }

            var result = sut.GetUserCount(lastActivityDateTimeUtc:cutOff);

            Assert.GreaterOrEqual(result, 15);
        }

        [Test]
        public void GetUserCount_UsesLastActivityDate_WayInThefuture()
        {
            var sut = this.GetRepository();

            DateTime cutOff = DateTime.MaxValue;
            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SrirachaUser>();
                sut.CreateUser(data);
            }

            var result = sut.GetUserCount(lastActivityDateTimeUtc: cutOff);

            Assert.AreEqual(0, result);
        }
    }
}
