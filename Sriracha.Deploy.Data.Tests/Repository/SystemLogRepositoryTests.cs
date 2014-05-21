using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository
{
    public abstract class SystemLogRepositoryTests : RepositoryTestBase<ISystemLogRepository>
    {
        private void AssertCreatedMessage(SystemLog expected, SystemLog actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else 
            {
                Assert.IsNotNull(actual);
                Assert.IsNotNullOrEmpty(actual.Id);
                Assert.AreEqual(expected.EnumSystemLogTypeID, actual.EnumSystemLogTypeID);
                Assert.AreEqual(expected.UserName, actual.UserName);
                AssertDateEqual(expected.MessageDateTimeUtc, actual.MessageDateTimeUtc);
                Assert.AreEqual(expected.MessageText, actual.MessageText);
                Assert.AreEqual(expected.LoggerName, actual.LoggerName);
            }
        }

        private void AssertMessage(SystemLog expected, SystemLog actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.EnumSystemLogTypeID, actual.EnumSystemLogTypeID);
                Assert.AreEqual(expected.UserName, actual.UserName);
                AssertDateEqual(expected.MessageDateTimeUtc, actual.MessageDateTimeUtc);
                Assert.AreEqual(expected.MessageText, actual.MessageText);
                Assert.AreEqual(expected.LoggerName, actual.LoggerName);
            }
        }

        [Test]
        public void LogMessage_LogsMessage()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemLog>();

            var result = sut.LogMessage(data.EnumSystemLogTypeID, data.UserName, data.MessageDateTimeUtc, data.MessageText, data.LoggerName);

            AssertCreatedMessage(data, result);
            var dbItem = sut.GetMessage(result.Id);
            AssertMessage(result, dbItem);
        }

        [Test]
        public void GetMessage_GetsMessage()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemLog>();
            var message = sut.LogMessage(data.EnumSystemLogTypeID, data.UserName, data.MessageDateTimeUtc, data.MessageText, data.LoggerName);

            var result = sut.GetMessage(message.Id);

            AssertMessage(message, result);
        }

        [Test]
        public void GetMessage_MissingID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetMessage(null));
        }

        [Test]
        public void GetMessage_BadID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetMessage(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetList_ReturnsList()
        {
            var sut = this.GetRepository();
            var messageList = new List<SystemLog>();
            for(int i = 0; i < 20; i++)
            {
                var data = this.Fixture.Create<SystemLog>();
                var message = sut.LogMessage(data.EnumSystemLogTypeID, data.UserName, data.MessageDateTimeUtc, data.MessageText, data.LoggerName);
                messageList.Add(message);
            }

            var result = sut.GetList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.LessOrEqual(20, result.Items.Count);
            Assert.LessOrEqual(20, result.TotalItemCount);
        }

        [Test, Explicit]
        public void PurgeLogMessages_PurgesMessages()
        {  
            var sut = this.GetRepository();
            var messageList = new List<SystemLog>();
            for (int i = 0; i < 20; i++)
            {
                var data = this.Fixture.Create<SystemLog>();
                var message = sut.LogMessage(EnumSystemLogType.Debug, data.UserName, DateTime.UtcNow.AddMinutes(-2), data.MessageText, data.LoggerName);
                messageList.Add(message);
            }

            sut.PurgeLogMessages(DateTime.UtcNow, EnumSystemLogType.Debug, 0);
            var result = sut.GetList(null, new List<EnumSystemLogType> { EnumSystemLogType.Debug });

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.TotalItemCount);
        }
    }
}
