using NUnit.Framework;
using Sriracha.Deploy.Data.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Build
{
    public abstract class FileStorageTests : RepositoryTestBase<IFileStorage>
    {
        [Test]
        public void StoreFile_StoresFile()
        {
            var sut = this.GetRepository();
            var fileData = this.Fixture.Create<byte[]>();

            var result = sut.StoreFile(fileData);

            Assert.IsNotNullOrEmpty(result);
            var dbData = sut.GetFile(result);
            Assert.AreEqual(fileData, dbData);
        }

        [Test]
        public void StoreFile_MissingFileData_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.StoreFile(null));
        }

        [Test]
        public void StoreFile_EmptyFileData_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.StoreFile(new byte[0]));
        }

        [Test]
        public void UpdateFile_UpdatesFile()
        {
            var sut = this.GetRepository();
            var initialData = this.Fixture.Create<byte[]>();
            var fileStorageId = sut.StoreFile(initialData);
            var updatedData = this.Fixture.Create<byte[]>();

            sut.UpdateFile(fileStorageId, updatedData);

            var dbData = sut.GetFile(fileStorageId);
            Assert.AreEqual(updatedData, dbData);
        }

        [Test]
        public void UpdateFile_MissingFileStorageID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var updatedData = this.Fixture.Create<byte[]>();

            Assert.Throws<ArgumentNullException>(() => sut.UpdateFile(null, updatedData));
        }

        [Test]
        public void UpdateFile_BadFileStorageID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            var updatedData = this.Fixture.Create<byte[]>();

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateFile(Guid.NewGuid().ToString(), updatedData));
        }

        [Test]
        public void GetFile_GetsFile()
        {
            var sut = this.GetRepository();
            var fileData = this.Fixture.Create<byte[]>();
            var fileStorageId = sut.StoreFile(fileData);

            var result = sut.GetFile(fileStorageId);

            Assert.IsNotNull(result);
            Assert.AreEqual(fileData, result);
        }

        [Test]
        public void GetFile_MissingFileStorageId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetFile(null));
        }

        [Test]
        public void GetFile_BadFileStorageId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetFile(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetFileStream_GetsFileStream()
        {
            var sut = this.GetRepository();
            var fileData = this.Fixture.Create<byte[]>();
            var fileStorageId = sut.StoreFile(fileData);

            using(var result = sut.GetFileStream(fileStorageId))
            {
                var resultBytes = TempStreamHelper.ReadAllBytes(result);
                Assert.AreEqual(fileData, resultBytes);
            }
        }

        [Test]
        public void GetFileStream_MissingFileStorageId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetFileStream(null));
        }

        [Test]
        public void GetFileStream_BadFileStorageId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetFileStream(Guid.NewGuid().ToString()));
        }

        [Test]
        public void DeleteFile_DeletesFile()
        {
            var sut = this.GetRepository();
            var fileData = this.Fixture.Create<byte[]>();
            var fileStorageId = sut.StoreFile(fileData);

            sut.DeleteFile(fileStorageId);

            Assert.Throws<RecordNotFoundException>(() => sut.GetFile(fileStorageId));
        }

        [Test]
        public void DeleteFile_MissingFileStorageId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.DeleteFile(null));
        }


        [Test]
        public void DeleteFile_BadFileStorageId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteFile(Guid.NewGuid().ToString()));
        }
    }
}
