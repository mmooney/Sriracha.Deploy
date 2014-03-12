using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Build
{
    public class FileManifest
    {
        public string Id { get; set; }
        public List<FileManifestItem> ItemList { get; set; }

        public FileManifest()
        {
            this.ItemList = new List<FileManifestItem>();
        }
    }
}
