using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Impl;

namespace Sriracha.Deploy.Data.Tests
{
	public class BuildParameterEvaluatorTests
	{
		[Test]
		public void NullParameter_ThrowsArgumentNullException()
		{
			var sut = new BuildParameterEvaluator();

			Assert.Throws<ArgumentNullException>(()=>sut.Evaluate(null, new DeployBuild()));
		}

		[Test]
		public void NullBuild_ThrowsArgumentNullException()
		{
			var sut = new BuildParameterEvaluator();

			Assert.Throws<ArgumentNullException>(()=>sut.Evaluate(Guid.NewGuid().ToString(), null));
		}

		[Test]
		public void InvalidParameterName_ThrowsArgumentException()
		{
			var sut = new BuildParameterEvaluator();

			Assert.Throws<ArgumentException>(()=>sut.Evaluate(Guid.NewGuid().ToString(), new DeployBuild()));
		}

		[Test]
		public void Version()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = fixture.Create<string>()
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("Version", build);

			Assert.AreEqual(build.Version, result);
		}

		[Test]
		public void MajorVersion()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123.456.789.012"
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("MajorVersion", build);

			Assert.AreEqual("123", result);
		}

		[Test]
		public void MinorVersion()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123.456.789.012"
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("MinorVersion", build);

			Assert.AreEqual("456", result);
		}

		[Test]
		public void BuildVersion()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123.456.789.012"
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("BuildVersion", build);

			Assert.AreEqual("789", result);
		}

		[Test]
		public void RevisionVersion()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123.456.789.012"
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("RevisionVersion", build);

			Assert.AreEqual("012", result);
		}

		[Test]
		public void NoDots_MajorVersion_EntireString()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123456789012"
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("MajorVersion", build);

			Assert.AreEqual("123456789012", result);
		}

		[Test]
		public void NoDots_MinorVersion_ThrowsArgumentException()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123456789012"
			};
			var sut = new BuildParameterEvaluator();

			Assert.Throws<ArgumentException>(()=>sut.Evaluate("MinorVersion", build));
		}

		[Test]
		public void NoDots_BuildVersion_ThrowsArgumentException()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123456789012"
			};
			var sut = new BuildParameterEvaluator();

			Assert.Throws<ArgumentException>(() => sut.Evaluate("BuildVersion", build));
		}

		[Test]
		public void NoDots_RevisionVersion_ThrowsArgumentException()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123456789012"
			};
			var sut = new BuildParameterEvaluator();

			Assert.Throws<ArgumentException>(() => sut.Evaluate("RevisionVersion", build));
		}

		[Test]
		public void CaseInsensitive()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = fixture.Create<string>()
			};
			var sut = new BuildParameterEvaluator();

			var result = sut.Evaluate("veRsIon", build);

			Assert.AreEqual(build.Version, result);
		}
	}
}
