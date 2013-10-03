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

		//public string GetLocalMachineDirectory(string machineName)
		//{	
		//	if(string.IsNullOrEmpty(this.LocalDeployDirectory))
		//	{
		//		throw new Exception("Missing LocalDeployDirectory");
		//	}
		//	return Path.Combine(this.LocalDeployDirectory, "Machines", machineName);
		//}

		public string GetLocalMachineComponentDirectory(string machineName, string componentId)
		{
			if (string.IsNullOrEmpty(this.LocalDeployDirectory))
			{
				throw new Exception("Missing LocalDeployDirectory");
			}
			return Path.Combine(this.LocalDeployDirectory, componentId, machineName);
		}

		public string GetLocalCompressedPackageDirectory(string componentId)
		{
			if (string.IsNullOrEmpty(this.LocalDeployDirectory))
			{
				throw new Exception("Missing LocalDeployDirectory");
			}
			return Path.Combine(this.LocalDeployDirectory, componentId, "Compressed");
		}

		public string GetLocalExtractedDirectory(string componentId)
		{
			if (string.IsNullOrEmpty(this.LocalDeployDirectory))
			{
				throw new Exception("Missing LocalDeployDirectory");
			}
			return Path.Combine(this.LocalDeployDirectory, componentId, "Extracted");
		}
	}
}
