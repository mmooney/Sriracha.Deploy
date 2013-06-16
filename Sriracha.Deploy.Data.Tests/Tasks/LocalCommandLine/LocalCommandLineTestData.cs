using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks.LocalCommandLine;

namespace Sriracha.Deploy.Data.Tests.Tasks.LocalCommandLine
{
	public class LocalCommandLineTestData
	{
		public LocalCommandLineTaskDefinition TaskDefinition { get; set; }
		public LocalCommandLineTaskExecutor TaskExecutor { get; set; }

		public static LocalCommandLineTestData Create()
		{
			var returnValue = new LocalCommandLineTestData();

			return returnValue;
		}
	}
}
