using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Exceptions;

namespace Sriracha.Deploy.Data.Tests.Repository.Build
{
    public abstract class BuildRepositoryTests : RepositoryTestBase<IBuildRepository>
    {
        private class CreateTestData
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string ComponentId { get; set; }
            public string ComponentName { get; set; }
            public string BranchId { get; set; }
            public string BranchName { get; set; }
            public string FileId { get; set; }
            public string Version { get; set; }
        }

        private CreateTestData GetCreateTestData()
        {
            return this.Fixture.Create<CreateTestData>();
        }

        private void AssertCreatedBuild(CreateTestData testData, DeployBuild result)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(testData.ProjectId, result.ProjectId);
            Assert.AreEqual(testData.ProjectName, result.ProjectName);
            Assert.AreEqual(testData.ComponentId, result.ProjectComponentId);
            Assert.AreEqual(testData.ComponentName, result.ProjectComponentName);
            Assert.AreEqual(testData.BranchId, result.ProjectBranchId);
            Assert.AreEqual(testData.BranchName, result.ProjectBranchName);
            Assert.AreEqual(testData.FileId, result.FileId);
            Assert.AreEqual(testData.Version, result.Version);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
        }

        private void AssertBuild(DeployBuild expected, DeployBuild actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ProjectName, actual.ProjectName);
            Assert.AreEqual(expected.ProjectComponentId, actual.ProjectComponentId);
            Assert.AreEqual(expected.ProjectComponentName, actual.ProjectComponentName);
            Assert.AreEqual(expected.ProjectBranchId, actual.ProjectBranchId);
            Assert.AreEqual(expected.ProjectBranchName, actual.ProjectBranchName);
            Assert.AreEqual(expected.FileId, actual.FileId);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedDateTimeUtc, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }

        [Test]
        public void CreateBuild_CreatesBuild()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            var result = sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version);

            AssertCreatedBuild(testData, result);

            var dbBuild = sut.GetBuild(result.Id);
            AssertBuild(result, dbBuild);
        }

        [Test]
        public void CreateBuild_MissingProjectId_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.ProjectId = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingProjectName_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.ProjectName = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingComponentId_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.ComponentId = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingComponentName_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.ComponentName = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingBranchId_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.BranchId = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingBranchName_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.BranchName = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingFileId_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.FileId = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_MissingVersion_Throws_ArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            testData.Version = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_DuplicateData_ThrowsDuplicateObjectException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            var firstItem = sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version);
            Assert.Throws<DuplicateObjectException<DeployBuild>>(()=>sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_DuplicateDataNewVersion_CreatesBuild()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            var firstItem = sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version);
            testData.Version = this.Fixture.Create<string>("Version");
            var secondItem = sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version);

            AssertCreatedBuild(testData, secondItem);
        }
    }
}
