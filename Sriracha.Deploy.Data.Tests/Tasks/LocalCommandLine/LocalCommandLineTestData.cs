using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Tasks.LocalCommandLine;

namespace Sriracha.Deploy.Data.Tests.Tasks.LocalCommandLine
{
	public class LocalCommandLineTestData
	{
		public LocalCommandLineTaskDefinition TaskDefinition { get; set; }
		public LocalCommandLineTaskExecutor TaskExecutor { get; set; }

		public static LocalCommandLineTestData Create()
		{
			var fixture = new Fixture();
			var returnValue = new LocalCommandLineTestData
			{
				TaskDefinition = new LocalCommandLineTaskDefinition
				{
					Options = new LocalCommandLineTaskOptions
					{
						ExecutablePath = fixture.Create<string>()
					}
				},
				TaskExecutor = new LocalCommandLineTaskExecutor()
			};
			return returnValue;
		}
	}
}
