using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Build.BuildImpl;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests
{
    public class FileManagerTests
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public Mock<IFileRepository> FileRepository { get; set; }
            public Mock<IFileWriter> FileWriter { get; set; }
            public Mock<IManifestBuilder> ManifestBuilder { get; set; }
            public IFileManager Sut { get; set; }

            public static TestData Create()
            {
                var fixture = new Fixture();
                var testData = new TestData
                {
                    Fixture = fixture,
                    FileRepository = new Mock<IFileRepository>(),
                    FileWriter = new Mock<IFileWriter>(),
                    ManifestBuilder = new Mock<IManifestBuilder>()
                };
                testData.Sut = new FileManager(testData.FileRepository.Object, testData.FileWriter.Object, testData.ManifestBuilder.Object);
                return testData;
            }

            public class GetFileList
            {
                [Test]
                public void ReturnsFileList()
                {
                    var testData = TestData.Create();
                    var fileList = testData.Fixture.CreateMany<DeployFile>();
                    testData.FileRepository.Setup(i=>i.GetFileList()).Returns(fileList);

                    var result = testData.Sut.GetFileList();

                    Assert.IsNotNull(result);
                    Assert.AreEqual(fileList, result);
                    testData.FileRepository.Verify(i=>i.GetFileList(), Times.Once());
                }
            }

            public class CreateFile
            {
                [Test]
                public void CreatesFile()
                {
                    var testData = TestData.Create();
                    var fileName = testData.Fixture.Create<string>("fileName");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    var fileManifest = testData.Fixture.Create<FileManifest>();
                    testData.FileRepository.Setup(i => i.CreateFile(fileName, fileData, fileManifest)).Returns(deployFile);
                    testData.ManifestBuilder.Setup(i=>i.BuildFileManifest(fileData)).Returns(fileManifest);

                    var result = testData.Sut.CreateFile(fileName, fileData);

                    Assert.IsNotNull(result);
                    Assert.AreEqual(deployFile, result);
                    testData.ManifestBuilder.Verify(i=>i.BuildFileManifest(fileData), Times.Once());
                    testData.FileRepository.Verify(i => i.CreateFile(fileName, fileData, fileManifest), Times.Once());
                }

                [Test]
                public void MissingFileName_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();
                    var fileData = testData.Fixture.Create<byte[]>();

                    Assert.Throws<ArgumentNullException>(()=>testData.Sut.CreateFile(null, fileData));

                    testData.FileRepository.Verify(i=>i.CreateFile(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }

                [Test]
                public void MissingFileData_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();
                    var fileName = testData.Fixture.Create<string>("fileName");

                    Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateFile(fileName, null));

                    testData.FileRepository.Verify(i => i.CreateFile(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }

                [Test]
                public void EmptyFileData_ThrowsArgumentException()
                {
                    var testData = TestData.Create();
                    var fileName = testData.Fixture.Create<string>("fileName");
                    var fileData = new byte[0];

                    Assert.Throws<ArgumentException>(() => testData.Sut.CreateFile(fileName, fileData));

                    testData.FileRepository.Verify(i => i.CreateFile(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }
            }

            public class UpdateFile
            {
                [Test]
                public void UpdatesFile()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileName = testData.Fixture.Create<string>("fileName");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    var fileManifest = testData.Fixture.Create<FileManifest>();
                    testData.ManifestBuilder.Setup(i => i.BuildFileManifest(fileData)).Returns(fileManifest);
                    testData.FileRepository.Setup(i => i.UpdateFile(fileId, fileName, fileData, fileManifest)).Returns(deployFile);

                    var result = testData.Sut.UpdateFile(fileId, fileName, fileData);

                    Assert.IsNotNull(result);
                    Assert.AreEqual(deployFile, result);
                    testData.ManifestBuilder.Verify(i=>i.BuildFileManifest(fileData), Times.Once());
                    testData.FileRepository.Verify(i => i.UpdateFile(fileId, fileName, fileData, fileManifest), Times.Once());
                }

                [Test]
                public void MissingFileId_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();
                    string fileId = null;//testData.Fixture.Create<string>("fileId");
                    var fileName = testData.Fixture.Create<string>("fileName");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    var fileManifest = testData.Fixture.Create<FileManifest>();
                    testData.ManifestBuilder.Setup(i => i.BuildFileManifest(fileData)).Returns(fileManifest);
                    testData.FileRepository.Setup(i => i.UpdateFile(fileId, fileName, fileData, fileManifest)).Returns(deployFile);

                    Assert.Throws<ArgumentNullException>(()=>testData.Sut.UpdateFile(fileId, fileName, fileData));

                    testData.FileRepository.Verify(i => i.UpdateFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }

                [Test]
                public void MissingFileName_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    string fileName = null;//testData.Fixture.Create<string>("fileName");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    var fileManifest = testData.Fixture.Create<FileManifest>();
                    testData.ManifestBuilder.Setup(i => i.BuildFileManifest(fileData)).Returns(fileManifest);
                    testData.FileRepository.Setup(i => i.UpdateFile(fileId, fileName, fileData, fileManifest)).Returns(deployFile);

                    Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateFile(fileId, fileName, fileData));

                    testData.FileRepository.Verify(i => i.UpdateFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }

                [Test]
                public void MissingFileData_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileName = testData.Fixture.Create<string>("fileName");
                    byte[] fileData = null;//testData.Fixture.Create<byte[]>();
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    var fileManifest = testData.Fixture.Create<FileManifest>();
                    testData.ManifestBuilder.Setup(i => i.BuildFileManifest(fileData)).Returns(fileManifest);
                    testData.FileRepository.Setup(i => i.UpdateFile(fileId, fileName, fileData, fileManifest)).Returns(deployFile);

                    Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateFile(fileId, fileName, fileData));

                    testData.FileRepository.Verify(i => i.UpdateFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }

                [Test]
                public void EmptyFileData_ThrowsArgumentException()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileName = testData.Fixture.Create<string>("fileName");
                    byte[] fileData = new byte[0];//testData.Fixture.Create<byte[]>();
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    var fileManifest = testData.Fixture.Create<FileManifest>();
                    testData.ManifestBuilder.Setup(i => i.BuildFileManifest(fileData)).Returns(fileManifest);
                    testData.FileRepository.Setup(i => i.UpdateFile(fileId, fileName, fileData, fileManifest)).Returns(deployFile);

                    Assert.Throws<ArgumentException>(() => testData.Sut.UpdateFile(fileId, fileName, fileData));

                    testData.FileRepository.Verify(i => i.UpdateFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<FileManifest>()), Times.Never());
                }
            }

            public class GetFile
            {
                [Test]
                public void GetsFile()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var deployFile = testData.Fixture.Create<DeployFile>();
                    testData.FileRepository.Setup(i=>i.GetFile(fileId)).Returns(deployFile);

                    var result = testData.Sut.GetFile(fileId);
                    
                    Assert.IsNotNull(result);
                    Assert.AreEqual(deployFile, result);
                    testData.FileRepository.Verify(i=>i.GetFile(fileId), Times.Once());
                }

                [Test]
                public void MissingFileId_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();

                    Assert.Throws<ArgumentNullException>(()=>testData.Sut.GetFile(null));

                    testData.FileRepository.Verify(i => i.GetFile(It.IsAny<string>()), Times.Never());
                }
            }

            public class GetFileData
            {
                [Test]
                public void GetsFileData()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileData = testData.Fixture.Create<byte[]>();
                    testData.FileRepository.Setup(i => i.GetFileData(fileId)).Returns(fileData);

                    var result = testData.Sut.GetFileData(fileId);

                    Assert.IsNotNull(result);
                    Assert.AreEqual(fileData, result);
                    testData.FileRepository.Verify(i => i.GetFileData(fileId), Times.Once());
                }

                [Test]
                public void MissingFileId_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();

                    Assert.Throws<ArgumentNullException>(() => testData.Sut.GetFileData(null));

                    testData.FileRepository.Verify(i => i.GetFileData(It.IsAny<string>()), Times.Never());
                }
            }

            public class GetFileDataStream
            {
                [Test]
                public void GetsFileDataStream()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileDataStream = new MemoryStream();
                    testData.FileRepository.Setup(i => i.GetFileDataStream(fileId)).Returns(fileDataStream);

                    var result = testData.Sut.GetFileDataStream(fileId);

                    Assert.IsNotNull(result);
                    Assert.AreEqual(fileDataStream, result);
                    testData.FileRepository.Verify(i => i.GetFileDataStream(fileId), Times.Once());
                }

                [Test]
                public void MissingFileId_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();

                    Assert.Throws<ArgumentNullException>(() => testData.Sut.GetFileDataStream(null));

                    testData.FileRepository.Verify(i => i.GetFileDataStream(It.IsAny<string>()), Times.Never());
                }
            }

            public class DeleteFile
            {
                [Test]
                public void DeletesFile()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");

                    testData.Sut.DeleteFile(fileId);

                    testData.FileRepository.Verify(i=>i.DeleteFile(fileId), Times.Once());
                }

                [Test]
                public void MissingFileId_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();

                    Assert.Throws<ArgumentNullException>(() => testData.Sut.DeleteFile(null));

                    testData.FileRepository.Verify(i => i.DeleteFile(It.IsAny<string>()), Times.Never());
                }
            }

            public class ExportFile
            {
                [Test]
                public void ExportsFile()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var targetFilePath = testData.Fixture.Create<string>("targetFilePath");
                    testData.FileRepository.Setup(i=>i.GetFileData(fileId)).Returns(fileData);

                    testData.Sut.ExportFile(fileId, targetFilePath);

                    testData.FileWriter.Verify(i=>i.WriteBytes(targetFilePath, fileData), Times.Once());
                }

                [Test]
                public void MissingFileId_ThrowsArgumentNullException()
                {
                    var testData = TestData.Create();
                    string fileId = null;//testData.Fixture.Create<string>("fileId");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var targetFilePath = testData.Fixture.Create<string>("targetFilePath");

                    Assert.Throws<ArgumentNullException>(()=>testData.Sut.ExportFile(fileId, targetFilePath));

                    testData.FileWriter.Verify(i => i.WriteBytes(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never());
                }

                [Test]
                public void BadFileId_ThrowsArgumentException()
                {
                    var testData = TestData.Create();
                    var fileId = testData.Fixture.Create<string>("fileId");
                    var fileData = testData.Fixture.Create<byte[]>();
                    var targetFilePath = testData.Fixture.Create<string>("targetFilePath");
                    testData.FileRepository.Setup(i => i.GetFileData(fileId)).Returns<byte[]>(null);

                    Assert.Throws<ArgumentException>(()=>testData.Sut.ExportFile(fileId, targetFilePath));

                    testData.FileWriter.Verify(i => i.WriteBytes(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never());
                }
            }
        }
    }
}
