using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Build.BuildImpl;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Tests
{
    public partial class BuildManagerTests
    {
		public class CreateBuild
		{
			private class TestData
			{
				public Mock<IFileRepository> FileRepository { get; set; }
				public Mock<IBuildRepository> BuildRepository { get; set; }
				public Mock<IProjectRepository> ProjectRepository { get; set; }
				public Mock<IProjectNotifier> ProjectNotifier { get; set; }
				public BuildManager Sut { get; set; }
				public byte[] FileData { get; set; }
				public DeployProject DeployProject { get; set; }
				public DeployFile DeployFile { get; set; }
				public DeployBuild DeployBuild { get; set; }

				public static TestData Create()
				{
					Version v = new Version(TestDataHelper.RandomInt(0, 500), TestDataHelper.RandomInt(0, 500), TestDataHelper.RandomInt(0, 500), TestDataHelper.RandomInt(0, 500));
					var testData = new TestData
					{
						FileRepository = new Mock<IFileRepository>(),
						BuildRepository = new Mock<IBuildRepository>(),
						ProjectRepository = new Mock<IProjectRepository>(),
						ProjectNotifier = new Mock<IProjectNotifier>(),
						FileData = TempTestDataHelper.RandomBytes(1024),
						DeployFile = new DeployFile
						{
							Id = Guid.NewGuid().ToString(),
							FileName = Guid.NewGuid().ToString()
						},
						DeployBuild = new DeployBuild
						{
							Id = Guid.NewGuid().ToString(),
							ProjectId = Guid.NewGuid().ToString(),
							ProjectName = Guid.NewGuid().ToString(),
							ProjectComponentId = Guid.NewGuid().ToString(),
							ProjectComponentName = Guid.NewGuid().ToString(),
							ProjectBranchId = Guid.NewGuid().ToString(),
							ProjectBranchName = Guid.NewGuid().ToString(),
							Version = Guid.NewGuid().ToString()
						}
					};
					testData.DeployProject = new DeployProject
					{
						Id = testData.DeployBuild.ProjectId,
						ProjectName = testData.DeployBuild.ProjectName,
						BranchList = new List<DeployProjectBranch>
						{
							new DeployProjectBranch
							{
								Id = testData.DeployBuild.ProjectBranchId,
								ProjectId = testData.DeployBuild.ProjectId,
								BranchName = testData.DeployBuild.ProjectBranchName
							}
						},
						ComponentList = new List<DeployComponent>
						{
							new DeployComponent
							{
								Id = testData.DeployBuild.ProjectComponentId,
								ComponentName = testData.DeployBuild.ProjectComponentName
							}
						}
					};

					testData.DeployBuild.FileId = testData.DeployFile.Id;

					testData.FileRepository.Setup(i => i.CreateFile(testData.DeployFile.FileName, testData.FileData)).Returns(testData.DeployFile);

					testData.BuildRepository.Setup(i => i.CreateBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectName, testData.DeployBuild.ProjectComponentId, testData.DeployBuild.ProjectComponentName, testData.DeployBuild.ProjectBranchId, testData.DeployBuild.ProjectBranchName, testData.DeployBuild.FileId, testData.DeployBuild.Version)).Returns(testData.DeployBuild);

					testData.ProjectRepository.Setup(i => i.GetProject(testData.DeployBuild.ProjectId)).Returns(testData.DeployProject);
					testData.ProjectRepository.Setup(i => i.TryGetProject(testData.DeployBuild.ProjectId)).Returns(testData.DeployProject);
                    testData.ProjectRepository.Setup(i => i.GetBranch(testData.DeployBuild.ProjectBranchId, testData.DeployProject.Id)).Returns(testData.DeployProject.BranchList[0]);
                    testData.ProjectRepository.Setup(i => i.TryGetBranch(testData.DeployBuild.ProjectBranchId, testData.DeployProject.Id)).Returns(testData.DeployProject.BranchList[0]);
                    testData.ProjectRepository.Setup(i => i.GetComponent(testData.DeployBuild.ProjectComponentId, testData.DeployProject.Id)).Returns(testData.DeployProject.ComponentList[0]);
                    testData.ProjectRepository.Setup(i => i.TryGetComponent(testData.DeployBuild.ProjectComponentId, testData.DeployProject.Id)).Returns(testData.DeployProject.ComponentList[0]);

					testData.Sut = new BuildManager(testData.BuildRepository.Object, testData.FileRepository.Object, testData.ProjectRepository.Object, testData.ProjectNotifier.Object);

					return testData;
				}
			}

			[Test]
			public void CreateBuild_StoresFile()
			{
				var testData = TestData.Create();

				var result = testData.Sut.CreateBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectComponentId, testData.DeployBuild.ProjectBranchId, testData.DeployFile.FileName, testData.FileData, testData.DeployBuild.Version);

				Assert.IsNotNull(result);
				Assert.AreEqual(testData.DeployFile.Id, result.FileId);
				testData.FileRepository.Verify(i => i.CreateFile(testData.DeployFile.FileName, testData.FileData), Times.Once());
			}

			[Test]
			public void CreateBuild_StoresBuildInfo()
			{
				var testData = TestData.Create();

				var result = testData.Sut.CreateBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectComponentId, testData.DeployBuild.ProjectBranchId, testData.DeployFile.FileName, testData.FileData, testData.DeployBuild.Version);

				Assert.IsNotNull(result);
				Assert.AreEqual(testData.DeployBuild.Id, result.Id);
				testData.BuildRepository.Verify(i => i.CreateBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectName, testData.DeployBuild.ProjectComponentId, testData.DeployBuild.ProjectComponentName, testData.DeployBuild.ProjectBranchId, testData.DeployBuild.ProjectBranchName, testData.DeployBuild.FileId, testData.DeployBuild.Version), Times.Once());
			}
		}
	}
}
