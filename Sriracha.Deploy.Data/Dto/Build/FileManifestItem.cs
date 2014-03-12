using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Build
{
    public class FileManifestItem
    {
        public string FileName { get; set; }
        public string Directory { get; set; }
        public long FileSizeBytes { get; set; }
        public FileAttributes Attributes { get; set; }  
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public DateTime AccessedDateTime { get; set; }
    }
}
