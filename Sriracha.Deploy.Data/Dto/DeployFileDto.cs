using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployFileDto : DeployFile
	{
		public byte[] FileData { get; set; }
	}
}
