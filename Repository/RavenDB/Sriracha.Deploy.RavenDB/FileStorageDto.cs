using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
	public class FileStorageDto
	{
		public string Id { get; set; }
		public byte[] FileData { get; set; }
	}
}
