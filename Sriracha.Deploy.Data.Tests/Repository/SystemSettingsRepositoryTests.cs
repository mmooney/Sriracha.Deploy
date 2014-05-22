using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;

namespace Sriracha.Deploy.Data.Tests.Repository
{
    public abstract class SystemSettingsRepositoryTests : RepositoryTestBase<ISystemSettingsRepository>
    {
        public enum EnumTest
        {
            Value1 = 1,
            Value2 = 2,
            Value3 = 3,
            value4 = 4
        }

        [Test]
        public void GetStringSetting_GetsSetting()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<string>("Value");
            var defaultValue = this.Fixture.Create<string>("DefaultValue");
            sut.SetStringSetting(key, value);

            var result = sut.GetStringSetting(key, defaultValue);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetStringSetting_MissingKey_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetStringSetting(null, null));
        }

        [Test]
        public void GetStringSetting_UnknownKey_GetsDefault()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<string>("Value");
            var defaultValue = this.Fixture.Create<string>("DefaultValue");

            var result = sut.GetStringSetting(key, defaultValue);
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void GetIntSetting_GetsIntSetting()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<int>();
            var defaultValue = this.Fixture.Create<int>();
            sut.SetIntSetting(key, value);

            var result = sut.GetIntSetting(key, defaultValue);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetIntSetting_MissingKey_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetIntSetting(null, 0));
        }

        [Test]
        public void GetIntSetting_UnknownKey_GetsDefault()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<int>();
            var defaultValue = this.Fixture.Create<int>();

            var result = sut.GetIntSetting(key, defaultValue);
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void GetIntSetting_NonIntValue_ThrowsFormatException()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<string>("Value");
            var defaultValue = this.Fixture.Create<int>();
            sut.SetStringSetting(key, value);

            Assert.Throws<FormatException>(() => sut.GetIntSetting(key, defaultValue));
        }

        [Test]
        public void GetIntSetting_IntString_GetsIntValue()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<int>();
            var defaultValue = this.Fixture.Create<int>();
            sut.SetStringSetting(key, value.ToString());

            var result = sut.GetIntSetting(key, defaultValue);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBoolSetting_GetsBoolSetting()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<bool>();
            var defaultValue = this.Fixture.Create<bool>();
            sut.SetBoolSetting(key, value);

            var result = sut.GetBoolSetting(key, defaultValue);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBoolSetting_MissingKey_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetBoolSetting(null, false));
        }

        [Test]
        public void GetBoolSetting_UnknownKey_GetsDefault()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<bool>();
            var defaultValue = this.Fixture.Create<bool>();

            var result = sut.GetBoolSetting(key, defaultValue);
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void GetBoolSetting_NonBoolValue_ThrowsFormatException()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<string>("Value");
            var defaultValue = this.Fixture.Create<bool>();
            sut.SetStringSetting(key, value);

            Assert.Throws<FormatException>(() => sut.GetBoolSetting(key, defaultValue));
        }

        [Test]
        public void GetBoolSetting_BoolString_GetsBoolValue()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<bool>();
            var defaultValue = this.Fixture.Create<bool>();
            sut.SetStringSetting(key, value.ToString());

            var result = sut.GetBoolSetting(key, defaultValue);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetEnumSetting_GetsBoolSetting()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<EnumTest>();
            var defaultValue = this.Fixture.Create<EnumTest>();
            sut.SetEnumSetting(key, value);

            var result = sut.GetEnumSetting(key, defaultValue);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetEnumSetting_MissingKey_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var defaultValue = this.Fixture.Create<EnumTest>();

            Assert.Throws<ArgumentNullException>(() => sut.GetEnumSetting(null, defaultValue));
        }

        [Test]
        public void GetEnumSetting_UnknownKey_GetsDefault()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<EnumTest>();
            var defaultValue = this.Fixture.Create<EnumTest>();

            var result = sut.GetEnumSetting(key, defaultValue);
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void GetEnumSetting_NonEnumValue_ThrowsFormatException()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<string>("Value");
            var defaultValue = this.Fixture.Create<EnumTest>();
            sut.SetStringSetting(key, value);

            Assert.Throws<FormatException>(() => sut.GetEnumSetting(key, defaultValue));
        }

        [Test]
        public void GetEnumSetting_EnumString_GetsBoolValue()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<EnumTest>();
            var defaultValue = this.Fixture.Create<EnumTest>();
            sut.SetStringSetting(key, value.ToString());

            var result = sut.GetEnumSetting(key, defaultValue);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetEnumSetting_EnumInt_GetsBoolValue()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<EnumTest>();
            var defaultValue = this.Fixture.Create<EnumTest>();
            sut.SetIntSetting(key, (int)value);

            var result = sut.GetEnumSetting(key, defaultValue);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void AnyActiveSettings_ActiveSettings_ReturnsTrue()
        {
            var sut = this.GetRepository();
            var key = this.Fixture.Create<string>("Key");
            var value = this.Fixture.Create<string>("Value");
            var defaultValue = this.Fixture.Create<string>("DefaultValue");
            sut.SetStringSetting(key, value);

            var result = sut.AnyActiveSettings();

            Assert.IsTrue(result);
        }

        [Test]
        public void AnyActiveSettings_NoActiveSettings_ReturnsFalse()
        {
            var sut = this.GetRepository();
            sut.InactivateActiveSettings();

            var result = sut.AnyActiveSettings();

            Assert.IsFalse(result);
        }

    }
}
