using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NLog;
using NUnit.Framework;
using Sriracha.Deploy.Data.Impl;

namespace Sriracha.Deploy.Data.Tests
{
	public class RegexResolverTests
	{
		private class TestClass
		{
			public string TestField1 { get; set; }
			public string TestField2 { get; set; }
		}
		[Test]
		public void DoNothing()
		{
			var dataObject = new TestClass
			{
				TestField1 = "TestField1",
				TestField2 = "TestField2"
			};

			var sut = new RegexResolver(new Mock<Logger>().Object);
			sut.ResolveValues(dataObject);

			Assert.AreEqual("TestField1", dataObject.TestField1);
			Assert.AreEqual("TestField2", dataObject.TestField2);
		}

		[Test]
		public void SimpleReplacement()
		{
			var dataObject = new TestClass
			{
				TestField1 = "TestField1234",
				TestField2 = @"TestField1<</(\d+)/"
			};

			var sut = new RegexResolver(new Mock<Logger>().Object);
			sut.ResolveValues(dataObject);

			Assert.AreEqual("TestField1234", dataObject.TestField1);
			Assert.AreEqual("1234", dataObject.TestField2);
		}

		[Test]
		public void CaseInsensitiveFieldName()
		{
			var dataObject = new TestClass
			{
				TestField1 = "TestField1234",
				TestField2 = @"testfield1<</(\d+)/"
			};

			var sut = new RegexResolver(new Mock<Logger>().Object);
			sut.ResolveValues(dataObject);

			Assert.AreEqual("TestField1234", dataObject.TestField1);
			Assert.AreEqual("1234", dataObject.TestField2);
		}

		[Test]
		public void ExtractComponentName()
		{
			var dataObject = new TestClass()
			{
				TestField1 = @"C:\Test\My.Object.1.2.3.4.zip",
				TestField2 = @"TestField1<</(?<=\\)([a-zA-Z\.]+)(?=\.\d)/"
			};

			var sut = new RegexResolver(new Mock<Logger>().Object);
			sut.ResolveValues(dataObject);

			Assert.AreEqual(@"C:\Test\My.Object.1.2.3.4.zip", dataObject.TestField1);
			Assert.AreEqual("My.Object", dataObject.TestField2);
		}

		[Test]
		public void ExtractVersionNumber()
		{
			var dataObject = new TestClass()
			{
				TestField1 = @"C:\Test\My.Object.1.2.3.4.zip",
				TestField2 = @"TestField1<</\d+(\.\d+)+/"
			};

			var sut = new RegexResolver(new Mock<Logger>().Object);
			sut.ResolveValues(dataObject);

			Assert.AreEqual(@"C:\Test\My.Object.1.2.3.4.zip", dataObject.TestField1);
			Assert.AreEqual("1.2.3.4", dataObject.TestField2);
		}
	}
}
