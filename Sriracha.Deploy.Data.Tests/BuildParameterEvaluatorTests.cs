using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Build.BuildImpl;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Deployment.DeploymentImpl;

namespace Sriracha.Deploy.Data.Tests
{
	public class BuildParameterEvaluatorTests
	{
		[Test]
		public void NullParameter_ThrowsArgumentNullException()
		{
			var sut = new ParameterEvaluator();

			Assert.Throws<ArgumentNullException>(()=>sut.EvaluateBuildParameter(null, new DeployBuild()));
		}

		[Test]
		public void NullBuild_ThrowsArgumentNullException()
		{
			var sut = new ParameterEvaluator();

			Assert.Throws<ArgumentNullException>(()=>sut.EvaluateBuildParameter(Guid.NewGuid().ToString(), null));
		}

		[Test]
		public void InvalidParameterName_ThrowsArgumentException()
		{
			var sut = new ParameterEvaluator();

			Assert.Throws<ArgumentException>(()=>sut.EvaluateBuildParameter(Guid.NewGuid().ToString(), new DeployBuild()));
		}

		[Test]
		public void Version()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = fixture.Create<string>()
			};
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("Version", build);

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
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("MajorVersion", build);

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
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("MinorVersion", build);

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
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("BuildVersion", build);

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
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("RevisionVersion", build);

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
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("MajorVersion", build);

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
			var sut = new ParameterEvaluator();

			Assert.Throws<ArgumentException>(()=>sut.EvaluateBuildParameter("MinorVersion", build));
		}

		[Test]
		public void NoDots_BuildVersion_ThrowsArgumentException()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123456789012"
			};
			var sut = new ParameterEvaluator();

			Assert.Throws<ArgumentException>(() => sut.EvaluateBuildParameter("BuildVersion", build));
		}

		[Test]
		public void NoDots_RevisionVersion_ThrowsArgumentException()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = "123456789012"
			};
			var sut = new ParameterEvaluator();

			Assert.Throws<ArgumentException>(() => sut.EvaluateBuildParameter("RevisionVersion", build));
		}

		[Test]
		public void CaseInsensitive()
		{
			var fixture = new Fixture();
			var build = new DeployBuild
			{
				Version = fixture.Create<string>()
			};
			var sut = new ParameterEvaluator();

			var result = sut.EvaluateBuildParameter("veRsIon", build);

			Assert.AreEqual(build.Version, result);
		}
	}
}
