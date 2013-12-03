using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Dto;
using MMDB.Shared;

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

        private DeployBuild CreateTestBuild(IBuildRepository sut, string projectId = null, string branchId = null, string componentId=null)
        {
            projectId = StringHelper.IsNullOrEmpty(projectId, this.Fixture.Create<string>());
            branchId = StringHelper.IsNullOrEmpty(branchId, this.Fixture.Create<string>());
            componentId = StringHelper.IsNullOrEmpty(componentId, this.Fixture.Create<string>());
            return sut.CreateBuild(projectId, this.Fixture.Create<string>("ProjectName"), componentId, this.Fixture.Create<string>("ComponentName"),
                                        branchId, this.Fixture.Create<string>("BranchName"), this.Fixture.Create<string>(), this.Fixture.Create<string>());
        }

        private void AssertUpdatedBuild(IBuildRepository sut, DeployBuild original, DeployBuild updated, string newProjectId, string newProjectName, string newComponentId, string newComponentName, string newBranchId, string newBranchName, string newFileId, string newVersion, string newUserName)
        {
            Assert.IsNotNull(updated);
            Assert.AreEqual(original.Id, updated.Id);
            Assert.AreEqual(newProjectId, updated.ProjectId);
            Assert.AreEqual(newProjectName, updated.ProjectName);
            Assert.AreEqual(newComponentId, updated.ProjectComponentId);
            Assert.AreEqual(newComponentName, updated.ProjectComponentName);
            Assert.AreEqual(newBranchId, updated.ProjectBranchId);
            Assert.AreEqual(newBranchName, updated.ProjectBranchName);
            Assert.AreEqual(newComponentId, updated.ProjectComponentId);
            Assert.AreEqual(newComponentName, updated.ProjectComponentName);
            Assert.AreEqual(newFileId, updated.FileId);
            Assert.AreEqual(newVersion, updated.Version);
            AssertDateEqual(original.CreatedDateTimeUtc, updated.CreatedDateTimeUtc);
            Assert.AreEqual(original.CreatedByUserName, updated.CreatedByUserName);
            AssertIsRecent(updated.UpdatedDateTimeUtc);
            Assert.AreEqual(newUserName, updated.UpdatedByUserName);

            var dbBuild = sut.GetBuild(original.Id);
            AssertBuild(updated, dbBuild);
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
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
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
            Assert.Throws<DuplicateObjectException<DeployBuild>>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
        }

        [Test]
        public void CreateBuild_DuplicateData_NewFileId_ThrowsDuplicateObjectException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData();
            var firstItem = sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version);
            testData.FileId = this.Fixture.Create<string>("FileId");
            Assert.Throws<DuplicateObjectException<DeployBuild>>(() => sut.CreateBuild(testData.ProjectId, testData.ProjectName, testData.ComponentId, testData.ComponentName, testData.BranchId, testData.BranchName, testData.FileId, testData.Version));
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

        [Test]
        public void GetBuildList_GetsBuildList()
        {
            var sut = this.GetRepository();

            for(int i = 0; i < 10; i++)
            {
                var build = this.CreateTestBuild(sut);
            }

            var result= sut.GetBuildList(null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreNotEqual(0, result.Items.Count);
            Assert.AreNotEqual(0, result.PageSize);
        }

        [Test]
        public void GetBuildList_Defaults()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(null);

            int defaultPageSize = 20;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(defaultPageSize, result.Items.Count);
            Assert.AreEqual(defaultPageSize, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(2, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsFalse(result.SortAscending);
            Assert.AreEqual("UpdatedDateTimeUtc", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetBuildList_PageSize()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize=5 });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(5, result.Items.Count);
            Assert.AreEqual(5, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(6, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsFalse(result.SortAscending);
            Assert.AreEqual("UpdatedDateTimeUtc", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetBuildList_SortByUpdatedDateTimeUtcAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { SortField = "UpdatedDateTimeUtc", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            DeployBuild lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.UpdatedDateTimeUtc, item.UpdatedDateTimeUtc);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetBuildList_SortByUpdatedDateTimeUtcDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { SortField = "UpdatedDateTimeUtc", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            DeployBuild lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.UpdatedDateTimeUtc, item.UpdatedDateTimeUtc);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetBuildList_SortByProjectNameAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { SortField = "ProjectName", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            DeployBuild lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.ProjectName, item.ProjectName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetBuildList_SortByProjectNameDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { SortField = "ProjectName", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            DeployBuild lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.ProjectName, item.ProjectName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetBuildList_SortByBranchNameAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { SortField = "ProjectBranchName", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            DeployBuild lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.ProjectBranchName, item.ProjectBranchName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetBuildList_SortByBranchNameDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { SortField = "ProjectBranchName", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            DeployBuild lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.ProjectBranchName, item.ProjectBranchName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetBuildList_PageNumber()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestBuild(sut);
            }

            var result = sut.GetBuildList(new ListOptions { PageNumber=2 });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.LessOrEqual(10, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            //Assert.IsTrue(buildList.HasNextPage);
            Assert.IsTrue(result.HasPreviousPage);
            //Assert.IsTrue(buildList.IsFirstPage);
            //Assert.IsFalse(buildList.IsLastPage);
            Assert.LessOrEqual(2, result.PageCount);
            Assert.AreEqual(2, result.PageNumber);
            Assert.IsFalse(result.SortAscending);
            Assert.AreEqual("UpdatedDateTimeUtc", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetBuildList_ProjectId()
        {
            var sut = this.GetRepository();

            string projectId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, projectId: projectId);
                buildList.Add(build);
            }
            string otherProjectId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, projectId: otherProjectId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize=buildList.Count }, projectId: projectId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(buildList.Count, result.Items.Count);
            Assert.AreEqual(buildList.Count, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsTrue(result.IsLastPage);
            Assert.AreEqual(1, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
        }

        [Test]
        public void GetBuildList_ProjectId_Page1()
        {
            var sut = this.GetRepository();

            string projectId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, projectId: projectId);
                buildList.Add(build);
            }
            string otherProjectId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, projectId: otherProjectId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = 20 }, projectId: projectId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(20, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
        }

        [Test]
        public void GetBuildList_ProjectId_Page2()
        {
            var sut = this.GetRepository();

            string projectId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, projectId: projectId);
                buildList.Add(build);
            }
            string otherProjectId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, projectId: otherProjectId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = 20, PageNumber=2 }, projectId: projectId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(10, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsTrue(result.HasPreviousPage);
            Assert.IsFalse(result.IsFirstPage);
            Assert.IsTrue(result.IsLastPage);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(2, result.PageNumber);
        }

        [Test]
        public void GetBuildList_BranchId()
        {
            var sut = this.GetRepository();

            string branchId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, branchId: branchId);
                buildList.Add(build);
            }
            string otherBranchId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, branchId: otherBranchId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = buildList.Count }, branchId: branchId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(buildList.Count, result.Items.Count);
            Assert.AreEqual(buildList.Count, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsTrue(result.IsLastPage);
            Assert.AreEqual(1, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
        }

        [Test]
        public void GetBuildList_BranchId_Page1()
        {
            var sut = this.GetRepository();

            string branchId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, branchId: branchId);
                buildList.Add(build);
            }
            string otherbranchId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, branchId: otherbranchId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = 20 }, branchId: branchId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(20, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
        }

        [Test]
        public void GetBuildList_BranchId_Page2()
        {
            var sut = this.GetRepository();

            string branchId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, branchId: branchId);
                buildList.Add(build);
            }
            string otherbranchId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, branchId: otherbranchId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = 20, PageNumber = 2 }, branchId: branchId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(10, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsTrue(result.HasPreviousPage);
            Assert.IsFalse(result.IsFirstPage);
            Assert.IsTrue(result.IsLastPage);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(2, result.PageNumber);
        }

        [Test]
        public void GetBuildList_ComponentId()
        {
            var sut = this.GetRepository();

            string componentId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, componentId: componentId);
                buildList.Add(build);
            }
            string otherComponentId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, componentId: otherComponentId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = buildList.Count }, componentId: componentId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(buildList.Count, result.Items.Count);
            Assert.AreEqual(buildList.Count, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsTrue(result.IsLastPage);
            Assert.AreEqual(1, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
        }

        [Test]
        public void GetBuildList_ComponentId_Page1()
        {
            var sut = this.GetRepository();

            string componentId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, componentId: componentId);
                buildList.Add(build);
            }
            string otherComponentId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, componentId: otherComponentId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = 20 }, componentId: componentId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(20, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
        }

        [Test]
        public void GetBuildList_ComponentId_Page2()
        {
            var sut = this.GetRepository();

            string componentId = this.Fixture.Create<string>();
            var buildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, componentId: componentId);
                buildList.Add(build);
            }
            string otherComponentId = this.Fixture.Create<string>();
            var otherBuildList = new List<DeployBuild>();
            for (int i = 0; i < 30; i++)
            {
                var build = this.CreateTestBuild(sut, componentId: otherComponentId);
                otherBuildList.Add(build);
            }

            var result = sut.GetBuildList(new ListOptions { PageSize = 20, PageNumber = 2 }, componentId: componentId);
            Assert.IsNotNull(buildList);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(10, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(buildList.Count, result.TotalItemCount);
            Assert.IsFalse(result.HasNextPage);
            Assert.IsTrue(result.HasPreviousPage);
            Assert.IsFalse(result.IsFirstPage);
            Assert.IsTrue(result.IsLastPage);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(2, result.PageNumber);
        }

        [Test]
        public void GetBuild_GetsBuild()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);

            var result = sut.GetBuild(build.Id);

            AssertBuild(build, result);
        }

        [Test]
        public void GetBuild_MissingBuildID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetBuild(null));
        }

        [Test]
        public void GetBuild_BadBuildID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetBuild(Guid.NewGuid().ToString()));
        }

        [Test]
        public void UpdateBuild_UpdatesBuild()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);

            var result = sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion);

            AssertUpdatedBuild(sut, build, result, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion, newUserName);
        }

        [Test]
        public void UpdateBuild_MissingBuildID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(null, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_BadBuildID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateBuild(Guid.NewGuid().ToString(), newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = null;//this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingProjectName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = null;//this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingBranchID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = null;//this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingBranchName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = null;//this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = null;//this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingComponentName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = null;//this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingFileID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = this.Fixture.Create<string>("Version");
            string newFileId = null;//this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void UpdateBuild_MissingVersion_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);
            string newProjectId = this.Fixture.Create<string>();
            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newBranchId = this.Fixture.Create<string>();
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newComponentId = this.Fixture.Create<string>();
            string newComponentName = this.Fixture.Create<string>("ComponentName");
            string newVersion = null;//this.Fixture.Create<string>("Version");
            string newFileId = this.Fixture.Create<string>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBuild(build.Id, newProjectId, newProjectName, newComponentId, newComponentName, newBranchId, newBranchName, newFileId, newVersion));
        }

        [Test]
        public void DeleteBuild_DeletesBuild()
        {
            var sut = this.GetRepository();

            var build = this.CreateTestBuild(sut);

            sut.DeleteBuild(build.Id);

            Assert.Throws<RecordNotFoundException>(()=>sut.GetBuild(build.Id));

            var buildList = sut.GetBuildList(null, projectId: build.ProjectId);
            Assert.AreEqual(0, buildList.TotalItemCount);
        }

        [Test]
        public void DeleteBuild_MissingBuildId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.DeleteBuild(null));
        }

        [Test]
        public void DeleteBuild_BadBuildId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteBuild(Guid.NewGuid().ToString()));
        }
    }
}
