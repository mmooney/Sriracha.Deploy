using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using NLog;

namespace Sriracha.Deploy.Data.Impl
{
	public class Zipper : IZipper
	{
		private readonly Logger _logger;
		public Zipper(Logger logger)
		{
			_logger = DIHelper.VerifyParameter(logger);
		}

		public void ZipDirectory(string directoryPath, string zipPath)
		{
			using(var zipFile = new ZipFile())
			{
				_logger.Debug("Zipping directory {0} to into file {1}", directoryPath, zipPath);
				zipFile.AddDirectory(directoryPath);
				zipFile.Save(zipPath);
				_logger.Debug("Done zipping directory {0} to into file {1}, {2} entries, {3} bytes", directoryPath, zipPath, zipFile.Count, new FileInfo(zipPath).Length);
			}
		}


		public void ZipFile(string filePath, string zipPath)
		{
			using (var zipFile = new ZipFile())
			{
				_logger.Debug("Zipping file {0} to into file {1}", filePath, zipPath);
				zipFile.AddFile(filePath);
				zipFile.Save(zipPath);
				_logger.Debug("Done zipping fule {0} to into file {1}, {2} entries, {3} bytes", filePath, zipPath, zipFile.Count, new FileInfo(zipPath).Length);
			}
		}
	}
}
