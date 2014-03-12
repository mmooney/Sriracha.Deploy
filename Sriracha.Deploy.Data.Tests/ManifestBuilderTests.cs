using Ionic.Zip;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Build.BuildImpl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests
{
    public class ManifestBuilderTests
    {
        private class ZipEntryData
        {
            public string FileName { get; set; }
            public string Directory { get; set; }
            public byte[] FileData { get; set; }
            public FileAttributes FileAttributes { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public DateTime ModifiedDateTime { get; set; }
            public DateTime AccessDateTime { get; set; }
        }
        [Test]
        public void RandomData_ReturnsSimpleManifest()
        {
            var sut = new ManifestBuilder();
            var fixture = new Fixture();
            var fileData = fixture.Create<byte[]>();

            var result = sut.BuildFileManifest(fileData);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result.ItemList);
        }

        [Test]
        public void ZipFile_ReturnsFileList()
        {
            var sut = new ManifestBuilder();
            var fixture = new Fixture();
            var dataList = fixture.CreateMany<ZipEntryData>();
            byte[] zipFileBytes;
            using(ZipFile file = new ZipFile())
            {
                foreach (var data in dataList)
                {
                    string fullFileName = data.Directory + "/" + data.FileName;
                    var entry = file.AddEntry(fullFileName, data.FileData);
                    entry.AccessedTime = data.AccessDateTime;
                    entry.CreationTime = data.CreatedDateTime;
                    entry.ModifiedTime = data.ModifiedDateTime;
                    entry.Attributes = data.FileAttributes;
                }
                using (var stream = new MemoryStream())
                {
                    file.Save(stream);
                    stream.Position = 0;
                    zipFileBytes = TempStreamHelper.ReadAllBytes(stream);
                }
            }

            var result = sut.BuildFileManifest(zipFileBytes);

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.ItemList);
            foreach(var data in dataList)
            {
                var entry = result.ItemList.SingleOrDefault(i=>i.FileName == data.FileName && i.Directory == data.Directory);
                Assert.IsNotNull(entry);
                Assert.AreEqual(data.AccessDateTime.ToUniversalTime(), entry.AccessedDateTime.ToUniversalTime());
                Assert.AreEqual(data.ModifiedDateTime.ToUniversalTime(), entry.ModifiedDateTime.ToUniversalTime());
                Assert.AreEqual(data.CreatedDateTime.ToUniversalTime(), entry.CreatedDateTime.ToUniversalTime());
                Assert.AreEqual(data.FileAttributes, entry.Attributes);
            }
        }
    }
}
