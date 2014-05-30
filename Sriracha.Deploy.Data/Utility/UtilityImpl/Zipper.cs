using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Common.Logging;

namespace Sriracha.Deploy.Data.Utility.UtilityImpl
{
	public class Zipper : IZipper
	{
		private readonly ILog _logger;

		public Zipper(ILog logger)
		{
			_logger = DIHelper.VerifyParameter(logger);
		}

		public void ZipDirectory(string directoryPath, string zipPath)
		{
			using(var zipFile = new ZipFile())
			{
				_logger.Debug(string.Format("Zipping directory {0} to into file {1}", directoryPath, zipPath));
				zipFile.AddDirectory(directoryPath);
				zipFile.Save(zipPath);
				_logger.Debug(string.Format("Done zipping directory {0} to into file {1}, {2} entries, {3} bytes", directoryPath, zipPath, zipFile.Count, new FileInfo(zipPath).Length));
			}
		}


		public void ZipFile(string filePath, string zipPath)
		{
			using (var zipFile = new ZipFile())
			{
				_logger.Debug(string.Format("Zipping file {0} to into file {1}", filePath, zipPath));
				zipFile.AddFile(filePath, string.Empty);
				zipFile.Save(zipPath);
				_logger.Debug(string.Format("Done zipping file {0} to into file {1}, {2} entries, {3} bytes", filePath, zipPath, zipFile.Count, new FileInfo(zipPath).Length));
			}
		}


		public void ExtractFile(string zipPath, string targetDirectory)
		{
			using(var zipFile = new ZipFile(zipPath))
			{
				_logger.Debug(string.Format("Extracting file {0} to directory {1}", zipPath, targetDirectory));
				zipFile.ExtractAll(targetDirectory);
				_logger.Debug(string.Format("Done extracting file {0} to to directory {1}", zipPath, targetDirectory));
			}
		}
	}
}
