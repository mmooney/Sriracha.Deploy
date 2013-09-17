using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IBuildPublisher
	{
		void PublishDirectory(string directoryPath, string apiUrl, string projectId, string componentId, string branchId, string version);
		void PublishFile(string filePath, string apiUrl, string projectId, string componentId, string branchId, string version, string newFileName);
	}
}
