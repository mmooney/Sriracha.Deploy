using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Utility.UtilityImpl;

namespace Sriracha.Deploy.Data.Tests
{
	public class ParameterParserTests
	{
		public class FindEnvironmentParameters
		{
			[Test]
			public void CanFindSingleEnvironmentParameter()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${ENV:testvalue} this is a test";

				var result = sut.FindEnvironmentParameters(data);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("testvalue", result[0]);
			}

			[Test]
			public void CanFindMultipleEnvironmentParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${ENV:testvalue} this is a test${ENV:testvalue2}...";

				var result = sut.FindEnvironmentParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IgnoresEnvironmentParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${ENV:testvalue} this ${MACHINE:ignoreMe} is a test${ENV:testvalue2}...";

				var result = sut.FindEnvironmentParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IsCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${env:testvalue} this is a test${EnV:testvalue2}...";

				var result = sut.FindEnvironmentParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void HandlesDuplicates()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${env:testvalue} this is a test${ENV:testvalue2} and again ${env:testvalue} ...";

				var result = sut.FindEnvironmentParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}


			[Test]
			public void HandlesDuplicatesCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${env:testVALUE} this is a test${env:testvalue2} and again ${env:TESTvalue} ...";

				var result = sut.FindEnvironmentParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testVALUE", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}
		}

		public class FindDeployParameters
		{
			[Test]
			public void CanFindSingleDeployParameter()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${DEPLOY:testvalue} this is a test";

				var result = sut.FindDeployParameters(data);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("testvalue", result[0]);
			}

			[Test]
			public void CanFindMultipleDeployParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${DEPLOY:testvalue} this is a test${DEPLOY:testvalue2}...";

				var result = sut.FindDeployParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IgnoresEnvironmentAndMachineParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${DEPLOY:testvalue} this ${ENV:ignoreMe} ${MACHINE:ignoreMe2} is a test${DEPLOY:testvalue2}...";

				var result = sut.FindDeployParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IsCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${deploy:testvalue} this is a test${dEPLOY:testvalue2}...";

				var result = sut.FindDeployParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void HandlesDuplicates()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${deploy:testvalue} this is a test${dEPLOY:testvalue2} and again ${deploy:testvalue} ...";

				var result = sut.FindDeployParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}


			[Test]
			public void HandlesDuplicatesCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${deploy:testVALUE} this is a test${dEPLOY:testvalue2} and again ${DEPLOY:TESTvalue} ...";

				var result = sut.FindDeployParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testVALUE", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}
		}

		public class FindBuildParameters
		{
			[Test]
			public void CanFindSingleMachineParameter()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${BUILD:testvalue} this is a test";

				var result = sut.FindBuildParameters(data);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("testvalue", result[0]);
			}

			[Test]
			public void CanFindMultipleMachineParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${BUILD:testvalue} this is a test${BUILD:testvalue2}...";

				var result = sut.FindBuildParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IgnoresEnvironmentAndMachineParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${BUILD:testvalue} this ${ENV:ignoreMe} ${MACHINE:ignoreMe2} is a test${BUILD:testvalue2}...";

				var result = sut.FindBuildParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IsCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${build:testvalue} this is a test${bUILD:testvalue2}...";

				var result = sut.FindBuildParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void HandlesDuplicates()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${build:testvalue} this is a test${bUILD:testvalue2} and again ${build:testvalue} ...";

				var result = sut.FindBuildParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}


			[Test]
			public void HandlesDuplicatesCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${build:testVALUE} this is a test${bUILD:testvalue2} and again ${build:TESTvalue} ...";

				var result = sut.FindBuildParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testVALUE", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}
		}

		public class FindMachineParameters
		{
			[Test]
			public void CanFindSingleMachineParameter()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${MACHINE:testvalue} this is a test";
			
				var result = sut.FindMachineParameters(data);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("testvalue", result[0]);
			}

			[Test]
			public void CanFindMultipleMachineParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${MACHINE:testvalue} this is a test${MACHINE:testvalue2}...";

				var result = sut.FindMachineParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IgnoresEnvironmentParameters()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${MACHINE:testvalue} this ${ENV:ignoreMe} is a test${MACHINE:testvalue2}...";

				var result = sut.FindMachineParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void IsCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${machine:testvalue} this is a test${mACHINE:testvalue2}...";

				var result = sut.FindMachineParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}

			[Test]
			public void HandlesDuplicates()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${machine:testvalue} this is a test${mACHINE:testvalue2} and again ${machine:testvalue} ...";

				var result = sut.FindMachineParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testvalue", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}


			[Test]
			public void HandlesDuplicatesCaseInsensitive()
			{
				var sut = new ParameterParser();
				string data = "This is a Test ${machine:testVALUE} this is a test${mACHINE:testvalue2} and again ${machine:TESTvalue} ...";

				var result = sut.FindMachineParameters(data);

				Assert.AreEqual(2, result.Count);
				Assert.AreEqual("testVALUE", result[0]);
				Assert.AreEqual("testvalue2", result[1]);
			}
		}
	}
}
