using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IZipper
	{
		void ZipDirectory(string directoryPath, string zipPath);

		void ZipFile(string filePath, string zipPath);
	}
}
