using MMDB.ConnectionSettings;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository
{
    public abstract class ConnectionSettingRepositoryTests : RepositoryTestBase<IConnectionSettingRepository>
    {
        private class TestConnectionSettingClass : ConnectionSettingBase
        {
            public string Value1 { get; set; }
            public int Value2 { get; set; }
            public bool Value3 { get; set; }
        }

        private void AssertItem(TestConnectionSettingClass expected, TestConnectionSettingClass actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Key, actual.Key);
            Assert.AreEqual(expected.Value1, actual.Value1);
            Assert.AreEqual(expected.Value2, actual.Value2);
            Assert.AreEqual(expected.Value3, actual.Value3);
        }

        [Test]
        public void Load_Loads()
        {
            var sut = this.GetRepository();
            var value = this.Fixture.Create<TestConnectionSettingClass>();
            sut.Save(value);

            var result = sut.Load<TestConnectionSettingClass>(value.Key);

            AssertItem(value, result);
        }

        [Test]
        public void Load_MissingKey_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.Load<TestConnectionSettingClass>(null));
        }

        [Test]
        public void Load_BadKey_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(()=>sut.Load<TestConnectionSettingClass>(Guid.NewGuid().ToString()));
        }

        [Test]
        public void Save_SavesNewItem()
        {
            var sut = this.GetRepository();
            var value = this.Fixture.Create<TestConnectionSettingClass>();

            var result = sut.Save(value);

            AssertItem(value, result);
            var dbItem = sut.Load<TestConnectionSettingClass>(value.Key);
            AssertItem(value, result);
        }

        [Test]
        public void Save_UpdatesItem()
        {
            var sut = this.GetRepository();
            var value1 = this.Fixture.Create<TestConnectionSettingClass>();
            sut.Save(value1);
            var value2 = this.Fixture.Create<TestConnectionSettingClass>();
            value2.Key = value1.Key;

            var result = sut.Save(value2);
            
            AssertItem(value2, result);
            var dbItem = sut.Load<TestConnectionSettingClass>(value1.Key);
            AssertItem(value2, result);
        }
    }
}
