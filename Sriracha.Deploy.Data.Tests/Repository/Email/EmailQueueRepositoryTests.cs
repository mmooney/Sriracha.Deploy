using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Email
{
    public abstract class EmailQueueRepositoryTests : RepositoryTestBase<IEmailQueueRepository>
    {
        private void AssertCreatedEmail(SrirachaEmailMessage expected, SrirachaEmailMessage actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else 
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Subject, actual.Subject);
                AssertHelpers.AssertStringList(expected.EmailAddressList, actual.EmailAddressList);
                Assert.AreEqual(expected.RazorView, actual.RazorView);
                Assert.AreEqual(expected.DataObject, actual.DataObject);
                Assert.AreEqual(EnumQueueStatus.New, actual.Status);
                AssertIsRecent(actual.QueueDateTimeUtc);
                Assert.IsNull(actual.StartedDateTimeUtc);
                AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
                Assert.IsNotNull(actual.RecipientResultList);
                Assert.AreEqual(0, actual.RecipientResultList.Count);
            }
        }

        private void AssertEmail(SrirachaEmailMessage expected, SrirachaEmailMessage actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Subject, actual.Subject);
                AssertHelpers.AssertStringList(expected.EmailAddressList, actual.EmailAddressList);
                Assert.AreEqual(expected.RazorView, actual.RazorView);
                //Assert.AreEqual(expected.DataObject, actual.DataObject);
                Assert.AreEqual(EnumQueueStatus.New, actual.Status);
                AssertDateEqual(expected.QueueDateTimeUtc, actual.QueueDateTimeUtc);
                AssertDateEqual(expected.StartedDateTimeUtc, actual.StartedDateTimeUtc);
                AssertHelpers.AssertBaseDto(expected, actual);
                Assert.IsNotNull(actual.RecipientResultList);
                Assert.AreEqual(0, actual.RecipientResultList.Count);
                AssertRecipientResultList(expected.RecipientResultList, actual.RecipientResultList);
            }
        }

        private void AssertRecipientResultList(List<SrirachaEmailMessage.SrirachaEmailMessageRecipientResult> expectedList, List<SrirachaEmailMessage.SrirachaEmailMessageRecipientResult> actualList)
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
                    var actualItem = expectedList.FirstOrDefault(i=>i.Id == expectedItem.Id);
                    AssertRecipientResult(expectedItem, actualItem);
                }
            }
        }

        private static void AssertRecipientResult(SrirachaEmailMessage.SrirachaEmailMessageRecipientResult expected, SrirachaEmailMessage.SrirachaEmailMessageRecipientResult actual)
        {
            Assert.AreEqual(expected.SrirachaEmailMessageId, actual.SrirachaEmailMessageId);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.Details, actual.Details);
            Assert.AreEqual(expected.StatusDateTimeUtc, actual.StatusDateTimeUtc);
            AssertHelpers.AssertBaseDto(expected, actual);
        }

        [Test]
        public void CreateMessage_CreatesMessage()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();

            var result = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);

            AssertCreatedEmail(data, result);
            var dbItem = sut.GetMessage(result.Id);
            AssertEmail(result, dbItem);
        }

        [Test]
        public void CreateMessage_MissingSubject_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();

            Assert.Throws<ArgumentNullException>(()=>sut.CreateMessage(null, data.EmailAddressList, data.DataObject, data.RazorView));
        }
        
        [Test]
        public void CreateMessage_MissingEmailAddressList_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();

            Assert.Throws<ArgumentNullException>(() => sut.CreateMessage(data.Subject, null, data.DataObject, data.RazorView));
        }

        [Test]
        public void CreateMessage_EmptyEmailAddressList_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();

            Assert.Throws<ArgumentNullException>(() => sut.CreateMessage(data.Subject, new List<string>(), data.DataObject, data.RazorView));
        }

        [Test]
        public void CreateMessage_MissingDataObject_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();

            Assert.Throws<ArgumentNullException>(() => sut.CreateMessage(data.Subject, data.EmailAddressList, null, data.RazorView));
        }


        [Test]
        public void CreateMessage_MissingRazorView_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();

            Assert.Throws<ArgumentNullException>(() => sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, null));
        }

        [Test]
        public void GetMessage_GetsMessage()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var message = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);

            var result = sut.GetMessage(message.Id);

            AssertEmail(message, result);
        }

        [Test]
        public void GetMessage_MissingID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetMessage(null));
        }

        [Test]
        public void GetMessage_BadID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetMessage(Guid.NewGuid().ToString()));
        }

        [Test]
        public void PopNextMessage_PopsMessage()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var existingItem = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.PopNextMessage();

            Assert.IsNotNull(result);
            Assert.AreEqual(EnumQueueStatus.InProcess, result.Status);
            AssertIsRecent(result.StartedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
        }

        [Test]
        public void PopNextBatchDeployment_NoDeployments_ReturnsNothing()
        {
            var sut = this.GetRepository();

            var result = sut.PopNextMessage();

            Assert.IsNull(result);
        }

        [Test]
        public void UpdateMessageStatus_UpdatesStatus()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var message = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);
            var newStatus = this.Fixture.Create<EnumQueueStatus>();
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);

            var result = sut.UpdateMessageStatus(message.Id, newStatus);

            Assert.AreEqual(message.Id, result.Id);
            Assert.AreEqual(newStatus, result.Status);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);

            var dbItem = sut.GetMessage(result.Id);
            AssertEmail(result, dbItem);
        }

        [Test]
        public void UpdateMessageStatus_MissingID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var newStatus = this.Fixture.Create<EnumQueueStatus>();

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateMessageStatus(null, newStatus));
        }


        [Test]
        public void UpdateMessageStatus_BadID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            var newStatus = this.Fixture.Create<EnumQueueStatus>();

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateMessageStatus(Guid.NewGuid().ToString(), newStatus));
        }

        [Test]
        public void AddReceipientResult_AddsReceipientResult()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var message = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);
            var status = this.Fixture.Create<EnumQueueStatus>();
            var emailAddress = data.EmailAddressList.First();
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);

            var result = sut.AddReceipientResult(message.Id, status, emailAddress);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, message.Id);
            AssertHelpers.AssertUpdatedBaseDto(message, result, newUserName);
            Assert.AreEqual(message.RecipientResultList.Count, result.RecipientResultList.Count);
            var item = result.RecipientResultList.FirstOrDefault(i=>i.EmailAddress == emailAddress);
            Assert.IsNotNull(item);
            Assert.IsNotNullOrEmpty(item.Id);
            Assert.AreEqual(status, item.Status);
            Assert.AreEqual(emailAddress, item.EmailAddress);
            Assert.IsNullOrEmpty(item.Details);
            AssertIsRecent(item.StatusDateTimeUtc);
            AssertHelpers.AssertCreatedBaseDto(item, newUserName);
        }

        [Test]
        public void AddReceipientResult_WithException_AddsReceipientResult_WithDetails()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var message = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);
            var status = this.Fixture.Create<EnumQueueStatus>();
            var emailAddress = data.EmailAddressList.First();
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            var err = this.Fixture.Create<Exception>();

            var result = sut.AddReceipientResult(message.Id, status, emailAddress, err);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, message.Id);
            AssertHelpers.AssertUpdatedBaseDto(message, result, newUserName);
            Assert.AreEqual(message.RecipientResultList.Count, result.RecipientResultList.Count);
            var item = result.RecipientResultList.FirstOrDefault(i => i.EmailAddress == emailAddress);
            Assert.IsNotNull(item);
            Assert.IsNotNullOrEmpty(item.Id);
            Assert.AreEqual(status, item.Status);
            Assert.AreEqual(emailAddress, item.EmailAddress);
            Assert.AreEqual(err.ToString(), item.Details);
            AssertIsRecent(item.StatusDateTimeUtc);
            AssertHelpers.AssertCreatedBaseDto(item, newUserName);
        }

        [Test]
        public void AddReceipientResult_MissingMessageID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var status = this.Fixture.Create<EnumQueueStatus>();
            var emailAddress = this.Fixture.Create<string>("EmailAddress");

            Assert.Throws<ArgumentNullException>(()=>sut.AddReceipientResult(null, status, emailAddress));
        }

        [Test]
        public void AddReceipientResult_BadMessageID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            var status = this.Fixture.Create<EnumQueueStatus>();
            var emailAddress = this.Fixture.Create<string>("EmailAddress");

            Assert.Throws<RecordNotFoundException>(() => sut.AddReceipientResult(Guid.NewGuid().ToString(), status, emailAddress));
        }

        [Test]
        public void AddReceipientResult_MissingEmailAddress_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var message = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);
            var status = this.Fixture.Create<EnumQueueStatus>();
            var emailAddress = this.Fixture.Create<string>("EmailAddress");

            Assert.Throws<ArgumentNullException>(() => sut.AddReceipientResult(message.Id, status, null));
        }

        [Test]
        public void AddReceipientResult_BadEmailAddress_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SrirachaEmailMessage>();
            var message = sut.CreateMessage(data.Subject, data.EmailAddressList, data.DataObject, data.RazorView);
            var status = this.Fixture.Create<EnumQueueStatus>();
            var emailAddress = this.Fixture.Create<string>("EmailAddress");

            Assert.Throws<RecordNotFoundException>(() => sut.AddReceipientResult(message.Id, status, Guid.NewGuid().ToString()));
        }
    }
}
