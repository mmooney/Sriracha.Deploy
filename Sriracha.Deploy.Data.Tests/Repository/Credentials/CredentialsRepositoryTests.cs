using Sriracha.Deploy.Data.Dto.Credentials;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Credentials
{
    public abstract class CredentialsRepositoryTests : RepositoryTestBase<ICredentialsRepository>
    {
        private class TestData
        {
            public List<DeployCredentials> DeployCredentialsList { get; set; }
            public ICredentialsRepository Sut { get; set; }

            public static TestData Create(CredentialsRepositoryTests tester, int initialCredentialCount=0)
            {
                var testData = new TestData
                {
                    DeployCredentialsList = new List<DeployCredentials>(),
                    Sut = tester.GetRepository()
                };
                for(var i = 0; i < initialCredentialCount; i++)
                {
                    var data = tester.Fixture.Create<DeployCredentials>();
                    var item = testData.Sut.CreateCredentials(data.Domain, data.UserName, data.EncryptedPassword);
                    testData.DeployCredentialsList.Add(item);
                }
                return testData;
            }
        }

        private void AssertCredentials(DeployCredentials expected, DeployCredentials actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else 
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.Domain, actual.Domain);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.EncryptedPassword, actual.EncryptedPassword);

                Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
                AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
                Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
                AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
            }
        }

        private void AssertCreatedCredentials(DeployCredentials expected, DeployCredentials actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.IsNotNullOrEmpty(actual.Id);
                Assert.AreEqual(expected.Domain, actual.Domain);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.EncryptedPassword, actual.EncryptedPassword);

                Assert.AreEqual(this.UserName, actual.CreatedByUserName);
                AssertIsRecent(actual.CreatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.UpdatedByUserName);
                AssertIsRecent(actual.UpdatedDateTimeUtc);
            }
        }

        private void AssertUpdatedCredentials(DeployCredentials original, DeployCredentials expected, DeployCredentials actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.Domain, actual.Domain);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.EncryptedPassword, actual.EncryptedPassword);

                Assert.AreEqual(original.CreatedByUserName, actual.CreatedByUserName);
                AssertDateEqual(original.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
                Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
                AssertIsRecent(actual.UpdatedDateTimeUtc);
            }
        }

        [Test]
        public void GetCredentialsList_GetsList()
        {
            var testData = TestData.Create(this, 10);

            var result = testData.Sut.GetCredentialsList(null);

            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.TotalItemCount, 10);
        }

        [Test]
        public void GetCredentialsList_DefaultSortUserNameAsc()
        {
            var testData = TestData.Create(this, 10);

            var result = testData.Sut.GetCredentialsList(null);

            Assert.IsNotNull(result);
            Assert.AreEqual("UserName", result.SortField);
            Assert.IsTrue(result.SortAscending);
            string lastUserName = null;
            foreach(var item in result.Items)
            {
                if(string.IsNullOrEmpty(lastUserName))
                {
                    lastUserName = item.UserName;
                }
                else 
                {
                    Assert.GreaterOrEqual(item.UserName, lastUserName);
                }
            }
        }

        [Test]
        public void GetCredentialsList_SortUserNameDesc()
        {
            var testData = TestData.Create(this, 10);

            var result = testData.Sut.GetCredentialsList(new ListOptions { SortField="UserName", SortAscending=false });

            Assert.IsNotNull(result);
            Assert.AreEqual("UserName", result.SortField);
            Assert.IsFalse(result.SortAscending);
            string lastUserName = null;
            foreach (var item in result.Items)
            {
                if (string.IsNullOrEmpty(lastUserName))
                {
                    lastUserName = item.UserName;
                }
                else
                {
                    Assert.LessOrEqual(item.UserName, lastUserName);
                }
            }
        }

        [Test]
        public void GetCredentialsList_SortDomainAsc()
        {
            var testData = TestData.Create(this, 10);

            var result = testData.Sut.GetCredentialsList(new ListOptions { SortField="Domain", SortAscending=true });

            Assert.IsNotNull(result);
            Assert.AreEqual("Domain", result.SortField);
            Assert.IsTrue(result.SortAscending);
            string lastDomain = null;
            foreach (var item in result.Items)
            {
                if (string.IsNullOrEmpty(lastDomain))
                {
                    lastDomain = item.Domain;
                }
                else
                {
                    Assert.GreaterOrEqual(item.Domain, lastDomain);
                }
            }
        }

        [Test]
        public void GetCredentialsList_SortDomainDesc()
        {
            var testData = TestData.Create(this, 10);

            var result = testData.Sut.GetCredentialsList(new ListOptions { SortField = "Domain", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.AreEqual("Domain", result.SortField);
            Assert.IsFalse(result.SortAscending);
            string lastDomain = null;
            foreach (var item in result.Items)
            {
                if (string.IsNullOrEmpty(lastDomain))
                {
                    lastDomain = item.Domain;
                }
                else
                {
                    Assert.LessOrEqual(item.Domain, lastDomain);
                }
            }
        }

        [Test]
        public void GetCredentials_GetsCredentials()
        {
            var testData = TestData.Create(this, 1);

            var result = testData.Sut.GetCredentials(testData.DeployCredentialsList[0].Id);

            Assert.IsNotNull(result);
            AssertCredentials(testData.DeployCredentialsList[0], result);
        }

        [Test]
        public void GetCredentials_MissingCredentialsID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.GetCredentials(null));
        }

        [Test]
        public void GetCredentials_BadCredentialsID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 1);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetCredentials(Guid.NewGuid().ToString()));
        }

        [Test]
        public void CreateCredentials_CreatesCredentials()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();

            var result = testData.Sut.CreateCredentials(data.Domain, data.UserName, data.EncryptedPassword);

            AssertCreatedCredentials(data, result);
            var dbItem = testData.Sut.GetCredentials(result.Id);
            AssertCredentials(result, dbItem);
        }

        [Test]
        public void CreateCredentials_MissingDomain_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.CreateCredentials(null, data.UserName, data.EncryptedPassword));
        }

        [Test]
        public void CreateCredentials_MissingUserName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateCredentials(data.Domain, null, data.EncryptedPassword));
        }

        [Test]
        public void CreateCredentials_MissingPassword_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateCredentials(data.Domain, data.UserName, null));
        }

        [Test]
        public void CreateCredentials_DuplicateDomainAndUserName_ThrowsArgumentException()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();
            var result1 = testData.Sut.CreateCredentials(data.Domain, data.UserName, data.EncryptedPassword);

            Assert.Throws<ArgumentException>(() => testData.Sut.CreateCredentials(data.Domain, data.UserName, Guid.NewGuid().ToString()));
        }

        [Test]
        public void CreateCredentials_DuplicateDomainDifferentUserName_CreatesCredentials()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();
            var result1 = testData.Sut.CreateCredentials(data.Domain, data.UserName, data.EncryptedPassword);

            var result2 = testData.Sut.CreateCredentials(data.Domain, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            Assert.IsNotNull(result2);
        }

        [Test]
        public void CreateCredentials_DuplicateUserNameDifferentDomain_CreatesCredential()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployCredentials>();
            var result1 = testData.Sut.CreateCredentials(data.Domain, data.UserName, data.EncryptedPassword);

            var result2 = testData.Sut.CreateCredentials(Guid.NewGuid().ToString(), data.UserName, Guid.NewGuid().ToString());

            Assert.IsNotNull(result2);
        }

        [Test]
        public void UpdateCredentials_UpdatesCredentials()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            var result = testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, data.UserName, data.EncryptedPassword);

            AssertUpdatedCredentials(testData.DeployCredentialsList[0], data, result);
        }

        [Test]
        public void UpdateCredentials_MissingCredentialsID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.UpdateCredentials(null, data.Domain, data.UserName, data.EncryptedPassword));
        }

        [Test]
        public void UpdateCredentials_MissingDomain_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, null, data.UserName, data.EncryptedPassword));
        }

        [Test]
        public void UpdateCredentials_MissingUserName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, null, data.EncryptedPassword));
        }

        [Test]
        public void UpdateCredentials_MissingPassword_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, data.UserName, null));
        }


        [Test]
        public void UpdateCredentials_SameDomainAndUserName_UpdatesCredentials()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            data.UserName = testData.DeployCredentialsList[0].UserName;
            data.Domain = testData.DeployCredentialsList[0].Domain;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            var result = testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, data.UserName, data.EncryptedPassword);

            AssertUpdatedCredentials(testData.DeployCredentialsList[0], data, result);
        }

        [Test]
        public void UpdateCredentials_DuplicateDomainAndUserName_ThrowsArgumentException()
        {
            var testData = TestData.Create(this, 2);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            data.UserName = testData.DeployCredentialsList[1].UserName;
            data.Domain = testData.DeployCredentialsList[1].Domain;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            Assert.Throws<ArgumentException>(()=>testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, data.UserName, data.EncryptedPassword));
        }

        [Test]
        public void UpdateCredentials_DuplicateDomainDifferentUserName_UpdatesCredentials()
        {
            var testData = TestData.Create(this, 2);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            data.Domain = testData.DeployCredentialsList[1].Domain;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            var result = testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, data.UserName, data.EncryptedPassword);

            AssertUpdatedCredentials(testData.DeployCredentialsList[0], data, result);
        }

        [Test]
        public void UpdateCredentials_DuplicateUserNameDifferentDomain_UpdatesCredentials()
        {
            var testData = TestData.Create(this, 2);
            var data = this.Fixture.Create<DeployCredentials>();
            data.Id = testData.DeployCredentialsList[0].Id;
            data.UserName = testData.DeployCredentialsList[1].UserName;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            data.UpdatedByUserName = newUserName;

            var result = testData.Sut.UpdateCredentials(testData.DeployCredentialsList[0].Id, data.Domain, data.UserName, data.EncryptedPassword);

            AssertUpdatedCredentials(testData.DeployCredentialsList[0], data, result);
        }

        [Test]
        public void DeleteCredentials_DeletesCredentials()
        {
            var testData = TestData.Create(this, 1);

            var result = testData.Sut.DeleteCredentials(testData.DeployCredentialsList[0].Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployCredentialsList[0].Id, result.Id);

            Assert.Throws<RecordNotFoundException>(()=>testData.Sut.GetCredentials(testData.DeployCredentialsList[0].Id));
        }

        [Test]
        public void DeleteCredentials_MissingCredentialsID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.DeleteCredentials(null));
        }

        [Test]
        public void DeleteCredentials_BadCredentialsID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.DeleteCredentials(Guid.NewGuid().ToString()));
        }
    }
}
