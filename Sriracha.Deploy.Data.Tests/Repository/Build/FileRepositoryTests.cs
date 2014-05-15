using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using MMDB.Shared;
using System.IO;

namespace Sriracha.Deploy.Data.Tests.Repository.Build
{
    public abstract class FileRepositoryTests : RepositoryTestBase<IFileRepository>
    {
        protected Mock<IFileStorage> FileStorage { get; private set; }

        private class TestData
        {
            public List<DeployFileDto> FileList { get; set; }
            public IFileRepository Sut { get; set; }
            
            public static TestData Create(FileRepositoryTests tester, int existingCount=0)
            {
                var testData = new TestData 
                {
                    FileList = new List<DeployFileDto>(),
                    Sut = tester.GetRepository()
                };
                for(int i = 0; i < existingCount; i++)
                {
                    var data = tester.CreateTestData();
                    var file = testData.Sut.CreateFile(data.FileName, data.FileData, data.Manifest);
                    var fileDto = AutoMapper.Mapper.Map(file, new DeployFileDto());
                    fileDto.FileData = data.FileData;
                    tester.FileStorage.Setup(j=>j.GetFile(fileDto.FileStorageId)).Returns(fileDto.FileData);
                    tester.FileStorage.Setup(j => j.GetFileStream(fileDto.FileStorageId))
                        .Returns(()=> 
                        {
                            var x = new MemoryStream(fileDto.FileData);
                            x.Position = 0;
                            return x;
                        });
                    testData.FileList.Add(fileDto);
                }
                return testData;
            }
        }

        [SetUp]
        public void FileRepositorySetUp()
        {
            this.FileStorage = new Mock<IFileStorage>();
            AutoMapper.Mapper.CreateMap<DeployFile, DeployFileDto>();
        }

        private void AssertFile(DeployFile expected, DeployFile actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                AssertHelpers.AssertBaseDto(expected, actual);
                Assert.AreEqual(expected.FileName, actual.FileName);
                Assert.AreEqual(expected.FileStorageId, actual.FileStorageId);
                AssertFileManifest(expected.Manifest, actual.Manifest);
            }
        }

        private void AssertFileManifest(FileManifest expected, FileManifest actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                if (expected.ItemList == null)
                {
                    Assert.IsNull(actual.ItemList);
                }
                else
                {
                    Assert.IsNotNull(actual.ItemList.Count);
                    foreach (var expectedItem in expected.ItemList)
                    {
                        var actualItem = actual.ItemList.FirstOrDefault(i => i.FileName == expectedItem.FileName && i.Directory == expectedItem.Directory);
                        Assert.IsNotNull(actualItem);
                        Assert.AreEqual(expectedItem.FileName, actualItem.FileName);
                        Assert.AreEqual(expectedItem.Directory, actualItem.Directory);
                        Assert.AreEqual(expectedItem.FileSizeBytes, actualItem.FileSizeBytes);
                        Assert.AreEqual(expectedItem.Attributes, actualItem.Attributes);
                        AssertDateEqual(expectedItem.FileCreatedDateTime, actualItem.FileCreatedDateTime);
                        AssertDateEqual(expectedItem.FileModifiedDateTime, actualItem.FileModifiedDateTime);
                        AssertDateEqual(expectedItem.FileAccessedDateTime, actualItem.FileAccessedDateTime);
                    }
                }
            }
        }

        private void AssertCreatedFile(DeployFileDto expected, DeployFile actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else 
            {
                AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
                Assert.AreEqual(expected.FileName, actual.FileName);
                Assert.AreEqual(expected.FileStorageId, actual.FileStorageId);
                AssertFileManifest(expected.Manifest, actual.Manifest);
            }
        }

        private void AssertUpdatedFile(DeployFile original, DeployFileDto expected, DeployFile actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                AssertHelpers.AssertUpdatedBaseDto(original, actual, this.UserName);
                Assert.AreEqual(expected.FileName, actual.FileName);
                Assert.AreEqual(expected.FileStorageId, actual.FileStorageId);
                AssertFileManifest(expected.Manifest, actual.Manifest);
            }
        }

        private DeployFileDto CreateTestData()
        {
            var data = this.Fixture.Create<DeployFileDto>();
            this.FileStorage.Setup(i => i.StoreFile(data.FileData)).Returns(data.FileStorageId);
            this.FileStorage.Setup(i => i.UpdateFile(It.IsAny<string>(), data.FileData));
            return data;
        }


        [Test]
        public void GetFileList_GetsFileList()
        {
            var testData = TestData.Create(this, 10);

            var result = testData.Sut.GetFileList();

            Assert.IsNotNull(result);
            Assert.LessOrEqual(10, result.Count);
        }


        [Test]
        public void CreateFile_CreatesFile()
        {
            var testData = TestData.Create(this);
            var data = CreateTestData();

            var result = testData.Sut.CreateFile(data.FileName, data.FileData, data.Manifest);
            
            Assert.IsNotNull(result);
            this.FileStorage.Verify(i=>i.StoreFile(data.FileData), Times.Once());
            AssertCreatedFile(data, result);
            var dbItem = testData.Sut.GetFile(result.Id);
            AssertFile(result, dbItem);
        }

        [Test]
        public void CreateFile_MissingFileName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = CreateTestData();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.CreateFile(null, data.FileData, data.Manifest));
        }

        [Test]
        public void CreateFile_MissingFileData_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = CreateTestData();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateFile(data.FileName, null, data.Manifest));
        }

        [Test]
        public void CreateFile_EmptyFileData_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = CreateTestData();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateFile(data.FileName, new byte[0] , data.Manifest));
        }

        [Test]
        public void CreateFile_MissingManifest_CreatesFile()
        {
            var testData = TestData.Create(this);
            var data = CreateTestData();
            data.Manifest = null;

            var result = testData.Sut.CreateFile(data.FileName, data.FileData, data.Manifest);

            Assert.IsNotNull(result);
            this.FileStorage.Verify(i => i.StoreFile(data.FileData), Times.Once());
            AssertCreatedFile(data, result);
            var dbItem = testData.Sut.GetFile(result.Id);
            AssertFile(result, dbItem);
        }

        [Test]
        public void GetFile_GetsFile()
        {
            var testData = TestData.Create(this, 1);

            var result = testData.Sut.GetFile(testData.FileList[0].Id);

            Assert.IsNotNull(result);
            AssertFile(testData.FileList[0], result);
        }

        [Test]
        public void GetFile_MissingFileID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.GetFile(null));
        }

        [Test]
        public void GetFile_BadFileID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetFile(Guid.NewGuid().ToString()));
        }

        [Test]
        public void UpdateFile_UpdatesFile()
        {
            var testData = TestData.Create(this, 1);
            var data = this.CreateTestData();
            data.FileStorageId = testData.FileList[0].FileStorageId;
            this.CreateNewUserName();

            var result = testData.Sut.UpdateFile(testData.FileList[0].Id, data.FileName, data.FileData, data.Manifest);

            AssertUpdatedFile(testData.FileList[0], data, result);
            var dbItem = testData.Sut.GetFile(result.Id);
            AssertFile(result, dbItem);
            this.FileStorage.Verify(i => i.UpdateFile(testData.FileList[0].FileStorageId, data.FileData), Times.Once());
        }

        [Test]
        public void UpdateFile_MissingFileID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.CreateTestData();
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.UpdateFile(null, data.FileName, data.FileData, data.Manifest));
        }

        [Test]
        public void UpdateFile_BadFileID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);
            var data = this.CreateTestData();
            this.CreateNewUserName();

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.UpdateFile(Guid.NewGuid().ToString(), data.FileName, data.FileData, data.Manifest));
        }

        [Test]
        public void UpdateFile_MissingFileName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.CreateTestData();
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateFile(testData.FileList[0].Id, null, data.FileData, data.Manifest));
        }

        [Test]
        public void UpdateFile_MissingFileData_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.CreateTestData();
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateFile(testData.FileList[0].Id, data.FileName, null, data.Manifest));
        }

        [Test]
        public void UpdateFile_EmptyFileData_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var data = this.CreateTestData();
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateFile(testData.FileList[0].Id, data.FileName, new byte[0], data.Manifest));
        }

        [Test]
        public void UpdateFile_EmptyFileData_UpdatesFile()
        {
            var testData = TestData.Create(this, 1);
            var data = this.CreateTestData();
            data.Manifest = null;
            data.FileStorageId = testData.FileList[0].FileStorageId;
            this.CreateNewUserName();

            var result = testData.Sut.UpdateFile(testData.FileList[0].Id, data.FileName, data.FileData, data.Manifest);

            AssertUpdatedFile(testData.FileList[0], data, result);
            var dbItem = testData.Sut.GetFile(result.Id);
            AssertFile(result, dbItem);
            this.FileStorage.Verify(i => i.UpdateFile(testData.FileList[0].FileStorageId, data.FileData), Times.Once());
        }

        [Test]
        public void DeleteFile_DeletesFile()
        {
            var testData = TestData.Create(this, 1);
            var data = this.CreateTestData();

            var result = testData.Sut.DeleteFile(testData.FileList[0].Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.FileList[0].Id, result.Id);
            Assert.Throws<RecordNotFoundException>(()=>testData.Sut.GetFile(testData.FileList[0].Id));
            this.FileStorage.Verify(i=>i.DeleteFile(testData.FileList[0].FileStorageId), Times.Once());
        }

        [Test]
        public void DeleteFile_MissingFileID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.CreateTestData();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.DeleteFile(null));
            this.FileStorage.Verify(i => i.DeleteFile(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void DeleteFile_BadFileID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);
            var data = this.CreateTestData();

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.DeleteFile(Guid.NewGuid().ToString()));
            this.FileStorage.Verify(i => i.DeleteFile(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void GetFileData_GetsFileData()
        {
            var testData = TestData.Create(this, 1);

            var result = testData.Sut.GetFileData(testData.FileList[0].Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.FileList[0].FileData, result);
        }

        [Test]
        public void GetFileData_MissingFileID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.GetFileData(null));
        }

        [Test]
        public void GetFileData_BadFileID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetFileData(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetFileStream_GetsFileStream()
        {
            var testData = TestData.Create(this, 1);

            using(var result = testData.Sut.GetFileDataStream(testData.FileList[0].Id))
            {
                Assert.IsNotNull(result);
                var resultBytes = TempStreamHelper.ReadAllBytes(result);
                Assert.AreEqual(testData.FileList[0].FileData, resultBytes);
            }
        }

        [Test]
        public void GetFileStream_MissingFileID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.GetFileDataStream(null));
        }

        [Test]
        public void GetFileStream_BadFileID_RecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetFileDataStream(Guid.NewGuid().ToString()));
        }
    }
}
