using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
	public class RuntimeSystemSettings
	{
		public string LocalDeployDirectory { get; set; }

		public string GetLocalMachineDirectory(string machineName)
		{	
			if(string.IsNullOrEmpty(this.LocalDeployDirectory))
			{
				throw new Exception("Missing LocalDeployDirectory");
			}
			return Path.Combine(this.LocalDeployDirectory, "Machines", machineName);
		}

		public string GetLocalCompressedPackageDirectory()
		{
			if (string.IsNullOrEmpty(this.LocalDeployDirectory))
			{
				throw new Exception("Missing LocalDeployDirectory");
			}
			return Path.Combine(this.LocalDeployDirectory, "Compressed");
		}

		public string GetLocalExtractedDirectory()
		{
			if (string.IsNullOrEmpty(this.LocalDeployDirectory))
			{
				throw new Exception("Missing LocalDeployDirectory");
			}
			return Path.Combine(this.LocalDeployDirectory, "Extracted");
		}
	}
}
