using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sriracha.Deploy.Data.Impl;

namespace Sriracha.Deploy.Data.Tests
{
	public interface TestInterface 
	{
		string Test(int input);
	}

	public class TestImplementationClass : TestInterface
	{
		public string Test(int input)
		{
 			return input.ToString();
		}
	}

	public class ModuleInspectorTests
	{
		public class FindTypesImplementingInterfaces
		{
			[Test]
			public void CanFindTypesImplementingInterfaces()
			{
				IModuleInspector sut = new ModuleInspector();
				var result = sut.FindTypesImplementingInterfaces(typeof(TestInterface));
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(typeof(TestImplementationClass), result[0]);
			}
		}
	}
}
