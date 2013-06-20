using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sriracha.Deploy.Data.Impl;

namespace Sriracha.Deploy.Data.Tests
{
	public class ProcessRunnerTests
	{
		[Test]
		public void Success()
		{
			string path = "C:\\windows\\system32\\tasklist.exe";
			string parameters = "/v";
			var sut = new ProcessRunner();
			using(StringWriter standardOutputWriter = new StringWriter())
			using(StringWriter errorOutputWriter = new StringWriter())
			{
				sut.Run(path, parameters, standardOutputWriter, errorOutputWriter);

				string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
				Assert.IsNotNullOrEmpty(standardOutput);

				string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
				Assert.IsNullOrEmpty(errorOutput);
			}
		}

		[Test]
		public void Error()
		{
			string path = "C:\\windows\\system32\\xcopy.exe";
			string parameters = "lksjdfsdf";
			var sut = new ProcessRunner();
			using (StringWriter standardOutputWriter = new StringWriter())
			using (StringWriter errorOutputWriter = new StringWriter())
			{
				sut.Run(path, parameters, standardOutputWriter, errorOutputWriter);

				string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
				Assert.IsNotNullOrEmpty(standardOutput);

				string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
				Assert.IsNullOrEmpty(errorOutput);
			}
		}
	}
}
