using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Build
{
	public class DeployFile : BaseDto
	{
		public string FileName { get; set; }
		public string FileStorageId { get; set; }
        public FileManifest Manifest { get; set; }
    }
}
