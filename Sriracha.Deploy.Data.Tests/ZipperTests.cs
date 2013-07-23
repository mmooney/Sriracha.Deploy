using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NLog;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Impl;

namespace Sriracha.Deploy.Data.Tests
{
	public class ZipperTests
	{
		[Test]
		public void ZipFile_ReleasesLockOnZipFile()
		{
			var fixture = new Fixture();
			string sourceFilePath = Path.GetTempFileName();
			string zipFilePath = Path.ChangeExtension(sourceFilePath, ".zip");
			File.WriteAllText(sourceFilePath, fixture.Create<string>());
			try 
			{

				var sut = new Zipper(new Mock<Logger>().Object);
				sut.ZipFile(sourceFilePath, zipFilePath);
				File.Delete(zipFilePath);
			}
			finally
			{
				try 
				{
					File.Delete(sourceFilePath);
				}
				catch {}

				try 
				{
					File.Delete(zipFilePath);
				}
				catch {}
			}
		}

		[Test]
		public void ZipDirectory_ReleasesLockOnZipFile()
		{
			var fixture = new Fixture();
			string sourceDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(sourceDirectory);
			string sourceFile1 = Path.Combine(sourceDirectory, Guid.NewGuid().ToString() + ".tmp");
			File.WriteAllText(sourceFile1, fixture.Create<string>());
			string sourceFile2 = Path.Combine(sourceDirectory, Guid.NewGuid().ToString() + ".tmp");
			File.WriteAllText(sourceFile2, fixture.Create<string>());

			string zipFilePath = Path.Combine(sourceDirectory, Guid.NewGuid().ToString() + ".zip");
			try
			{

				var sut = new Zipper(new Mock<Logger>().Object);
				sut.ZipDirectory(sourceDirectory, zipFilePath);
				File.Delete(zipFilePath);
			}
			finally
			{
				try 
				{
					Directory.Delete(sourceDirectory, true);
				}
				catch {}

				try
				{
					File.Delete(zipFilePath);
				}
				catch { }
			}
		}
	}
}
