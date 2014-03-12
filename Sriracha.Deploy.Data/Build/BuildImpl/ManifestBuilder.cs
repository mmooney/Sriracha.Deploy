using Ionic.Zip;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Build;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Build.BuildImpl
{
    public class ManifestBuilder : IManifestBuilder
    {
        public FileManifest BuildFileManifest(byte[] fileData)
        {
            var returnValue = new FileManifest();
            using(var stream = new MemoryStream(fileData))
            {
                if(ZipFile.IsZipFile(stream, false))
                {
                    stream.Position = 0;
                    using (var zipFile = ZipFile.Read(stream))
                    {
                        foreach(var entry in zipFile)
                        {
                            string fileName;
                            string directory;
                            if(entry.FileName.Contains("/"))
                            {
                                fileName = entry.FileName.Substring(entry.FileName.LastIndexOf('/')+1);
                                directory = entry.FileName.Substring(0, entry.FileName.LastIndexOf('/'));
                            }
                            else 
                            {
                                fileName = entry.FileName;
                                directory = null;
                            }
                            var item = new FileManifestItem
                            {
                                FileName = fileName,
                                Directory = directory,
                                FileSizeBytes = entry.UncompressedSize,
                                Attributes = entry.Attributes,
                                AccessedDateTime = entry.AccessedTime,
                                CreatedDateTime = entry.CreationTime,
                                ModifiedDateTime = entry.ModifiedTime
                            };
                            returnValue.ItemList.Add(item);
                        }
                    }
                }
            }
            return returnValue;
        }
    }
}
