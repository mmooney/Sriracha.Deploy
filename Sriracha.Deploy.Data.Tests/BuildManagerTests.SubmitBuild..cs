﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Tests
{
    public partial class BuildManagerTests
    {
		public class SubmitBuild
		{
			private class TestData
			{
				public Mock<IFileRepository> FileRepository { get; set; }
				public Mock<IBuildRepository> BuildRepository { get; set; }
				public Mock<IProjectRepository> ProjectRepository { get; set; }
				public IBuildManager Sut { get; set; }
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
							ProjectBranchId = Guid.NewGuid().ToString(),
							ProjectBranchName = Guid.NewGuid().ToString(),
							Version = new Version(TestDataHelper.RandomInt(0, 500), TestDataHelper.RandomInt(0, 500), TestDataHelper.RandomInt(0, 500), TestDataHelper.RandomInt(0, 500))
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
						}
					};

					testData.DeployBuild.FileId = testData.DeployFile.Id;

					testData.FileRepository.Setup(i => i.StoreFile(testData.DeployFile.FileName, testData.FileData)).Returns(testData.DeployFile);

					testData.BuildRepository.Setup(i => i.StoreBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectName, testData.DeployBuild.ProjectBranchId, testData.DeployBuild.ProjectBranchName, testData.DeployBuild.FileId, testData.DeployBuild.Version)).Returns(testData.DeployBuild);

					testData.ProjectRepository.Setup(i=>i.GetProject(testData.DeployBuild.ProjectId)).Returns(testData.DeployProject);
					testData.ProjectRepository.Setup(i=>i.GetBranch(testData.DeployProject, testData.DeployBuild.ProjectBranchId)).Returns(testData.DeployProject.BranchList[0]);

					testData.Sut = new BuildManager(testData.BuildRepository.Object, testData.FileRepository.Object, testData.ProjectRepository.Object);

					return testData;
				}
			}

			[Test]
			public void SubmitBuild_StoresFile()
			{
				var testData = TestData.Create();

				var result = testData.Sut.SubmitBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectBranchId, testData.DeployFile.FileName, testData.FileData, testData.DeployBuild.Version);

				Assert.IsNotNull(result);
				Assert.AreEqual(testData.DeployFile.Id, result.FileId);
				testData.FileRepository.Verify(i => i.StoreFile(testData.DeployFile.FileName, testData.FileData), Times.Once());
			}

			[Test]
			public void SubmitBuild_StoresBuildInfo()
			{
				var testData = TestData.Create();
			
				var result = testData.Sut.SubmitBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectBranchId, testData.DeployFile.FileName, testData.FileData, testData.DeployBuild.Version);

				Assert.IsNotNull(result);
				Assert.AreEqual(testData.DeployBuild.Id, result.Id);
				testData.BuildRepository.Verify(i => i.StoreBuild(testData.DeployBuild.ProjectId, testData.DeployBuild.ProjectName, testData.DeployBuild.ProjectBranchId, testData.DeployBuild.ProjectBranchName, testData.DeployBuild.FileId, testData.DeployBuild.Version), Times.Once());
			}
		}
	}
}
