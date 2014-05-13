using MMDB.Shared;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository.Build
{
    public abstract class BuildPurgeRuleRepositoryTests : RepositoryTestBase<IBuildPurgeRuleRepository>
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public List<BuildPurgeRule> SystemRules { get; set; }
            public List<BuildPurgeRule> ProjectRules { get; set; }
            public string ProjectId { get; set; }
            public IBuildPurgeRuleRepository Sut { get; set; }
            public static TestData Create(BuildPurgeRuleRepositoryTests tester, int existingSystemRuleCount = 0, int existingProjectRuleCount=0)
            {
                var fixture = new Fixture();
                var testData = new TestData()
                {
                    Fixture = new Fixture(),
                    SystemRules = new List<BuildPurgeRule>(),
                    ProjectRules = new List<BuildPurgeRule>(),
                    ProjectId = fixture.Create<string>("ProjectID")
                };
                testData.Sut = tester.GetRepository();

                for(int i = 0; i < existingSystemRuleCount; i++)
                {
                    var tempRule = fixture.Create<BuildPurgeRule>();
                    var newRule = testData.Sut.CreateSystemBuildPurgeRule(tempRule.BuildRetentionMinutes, tempRule.EnvironmentNameList, tempRule.EnvironmentIdList, tempRule.MachineNameList, tempRule.MachineIdList);
                    testData.SystemRules.Add(newRule);
                }
                for (int i = 0; i < existingProjectRuleCount; i++)
                {
                    var tempRule = fixture.Create<BuildPurgeRule>();
                    tempRule.ProjectId = testData.ProjectId;
                    var newRule = testData.Sut.CreateProjectBuildPurgeRule(tempRule.ProjectId, tempRule.BuildRetentionMinutes, tempRule.EnvironmentNameList, tempRule.EnvironmentIdList, tempRule.MachineNameList, tempRule.MachineIdList);
                    testData.ProjectRules.Add(newRule);
                }
                return testData;
            }

        }

        private void AssertRule(BuildPurgeRule expected, BuildPurgeRule actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else 
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.ProjectId, actual.ProjectId);
                Assert.AreEqual(expected.BuildRetentionMinutes, actual.BuildRetentionMinutes);
                AssertList(expected.EnvironmentIdList, actual.EnvironmentIdList);
                AssertList(expected.EnvironmentNameList, actual.EnvironmentNameList);
                AssertList(expected.MachineIdList, actual.MachineIdList);
                AssertList(expected.MachineNameList, actual.MachineNameList);
                AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
                Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
                AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
                Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            }
        }

        private void AssertCreatedRule(BuildPurgeRule expected, BuildPurgeRule actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.IsNotNullOrEmpty(actual.Id);
                Assert.AreEqual(expected.ProjectId, actual.ProjectId);
                Assert.AreEqual(expected.BuildRetentionMinutes, actual.BuildRetentionMinutes);
                AssertList(expected.EnvironmentIdList, actual.EnvironmentIdList);
                AssertList(expected.EnvironmentNameList, actual.EnvironmentNameList);
                AssertList(expected.MachineIdList, actual.MachineIdList);
                AssertList(expected.MachineNameList, actual.MachineNameList);
                AssertIsRecent(actual.CreatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.CreatedByUserName);
                AssertIsRecent(actual.UpdatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.UpdatedByUserName);
            }
        }

        private void AssertUpdatedRule(BuildPurgeRule original, BuildPurgeRule expected, BuildPurgeRule actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.ProjectId, actual.ProjectId);
                Assert.AreEqual(expected.BuildRetentionMinutes, actual.BuildRetentionMinutes);
                AssertList(expected.EnvironmentIdList, actual.EnvironmentIdList);
                AssertList(expected.EnvironmentNameList, actual.EnvironmentNameList);
                AssertList(expected.MachineIdList, actual.MachineIdList);
                AssertList(expected.MachineNameList, actual.MachineNameList);
                AssertDateEqual(original.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
                Assert.AreEqual(original.CreatedByUserName, actual.CreatedByUserName);
                AssertIsRecent(actual.UpdatedDateTimeUtc);
                Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            }
        }

        private void AssertList(List<string> expectedList, List<string> actualList)
        {
            if(expectedList == null)
            {
                Assert.IsNull(actualList);
            }
            else
            {
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedList.Count, actualList.Count);
                foreach(var expectedItem in expectedList)
                {
                    Assert.Contains(expectedItem, actualList);
                }
            }
        }

        [Test]
        public void GetSystemBuildPurgeRuleList_GetsList()
        {
            var testData = TestData.Create(this, 10, 0);

            var result = testData.Sut.GetSystemBuildPurgeRuleList();

            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.Count, 10);
        }

        [Test]
        public void GetSystemBuildPurgeRule_GetsItem()
        {
            var testData = TestData.Create(this, 1, 0);

            var result = testData.Sut.GetSystemBuildPurgeRule(testData.SystemRules[0].Id);

            AssertRule(testData.SystemRules[0], result);
        }

        [Test]
        public void GetSystemBuildPurgeRule_MissingRuleID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.GetSystemBuildPurgeRule(null));
        }

        [Test]
        public void GetSystemBuildPurgeRule_BadRuleID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetSystemBuildPurgeRule(Guid.NewGuid().ToString()));
        }

        [Test]
        public void CreateSystemBuildPurgeRule_CreatesRule()
        {
            var testData = TestData.Create(this);
            var ruleData = testData.Fixture.Create<BuildPurgeRule>();
            ruleData.ProjectId = null;

            var result = testData.Sut.CreateSystemBuildPurgeRule(ruleData.BuildRetentionMinutes, ruleData.EnvironmentNameList, ruleData.EnvironmentIdList, ruleData.MachineNameList, ruleData.MachineIdList);

            AssertCreatedRule(ruleData, result);
            var dbItem = testData.Sut.GetSystemBuildPurgeRule(result.Id);
            AssertRule(result, dbItem);
        }

        [Test]
        public void UpdateSystemBuildPurgeRule_UpdatesRule()
        {
            var testData = TestData.Create(this, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.SystemRules[0].Id;
            updatedRuleData.ProjectId = null;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            var result = testData.Sut.UpdateSystemBuildPurgeRule(updatedRuleData.Id, updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList);

            AssertUpdatedRule(testData.SystemRules[0], updatedRuleData, result);
        }

        [Test]
        public void UpdateSystemBuildPurgeRule_MissingRuleID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.SystemRules[0].Id;
            updatedRuleData.ProjectId = null;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.UpdateSystemBuildPurgeRule(null, updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList));
        }

        [Test]
        public void UpdateSystemBuildPurgeRule_BadRuleID_RecordNotFoundException()
        {
            var testData = TestData.Create(this, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.SystemRules[0].Id;
            updatedRuleData.ProjectId = null;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.UpdateSystemBuildPurgeRule(Guid.NewGuid().ToString(), updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList));
        }

        [Test]
        public void DeleteSystemBuildPurgeRule_DeletesItem()
        {
            var testData = TestData.Create(this, 1);

            var result = testData.Sut.DeleteSystemBuildPurgeRule(testData.SystemRules[0].Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.SystemRules[0].Id, result.Id);

            Assert.Throws<RecordNotFoundException>(()=>testData.Sut.GetSystemBuildPurgeRule(testData.SystemRules[0].Id));
        }

        [Test]
        public void DeleteSystemBuildPurgeRule_MissingRuleID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.DeleteSystemBuildPurgeRule(null));
        }

        [Test]
        public void DeleteSystemBuildPurgeRule_BadRuleID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.DeleteSystemBuildPurgeRule(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetProjectBuildPurgeRuleList_ReturnsList()
        {
            var testData = TestData.Create(this, 0, 10);
            
            var result = testData.Sut.GetProjectBuildPurgeRuleList(testData.ProjectId);

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Count);
        }

        [Test]
        public void GetProjectBuildPurgeRuleList_MissingProjectID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 10);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.GetProjectBuildPurgeRuleList(null));
        }

        [Test]
        public void GetProjectBuildPurgeRuleList_BadProjectID_ReturnsEmptyList()
        {
            var testData = TestData.Create(this);

            var result = testData.Sut.GetProjectBuildPurgeRuleList(Guid.NewGuid().ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetProjectBuildPurgeRule_GetsItem()
        {
            var testData = TestData.Create(this, 0, 1);

            var result = testData.Sut.GetProjectBuildPurgeRule(testData.ProjectRules[0].Id, testData.ProjectRules[0].ProjectId);

            Assert.IsNotNull(result);
            AssertRule(testData.ProjectRules[0], result);
        }

        [Test]
        public void GetProjectBuildPurgeRule_MissingRuleID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.GetProjectBuildPurgeRule(null, testData.ProjectRules[0].ProjectId));
        }

        [Test]
        public void GetProjectBuildPurgeRule_BadRuleID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetProjectBuildPurgeRule(Guid.NewGuid().ToString(), testData.ProjectRules[0].ProjectId));
        }

        [Test]
        public void GetProjectBuildPurgeRule_MissingProjectID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.GetProjectBuildPurgeRule(testData.ProjectRules[0].Id, null));
        }

        [Test]
        public void GetProjectBuildPurgeRule_BadProjectID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetProjectBuildPurgeRule(testData.ProjectRules[0].Id, Guid.NewGuid().ToString()));
        }

        [Test]
        public void CreateProjectBuildPurgeRule_CreatesRule()
        {
            var testData = TestData.Create(this);
            var ruleData = testData.Fixture.Create<BuildPurgeRule>();

            var result = testData.Sut.CreateProjectBuildPurgeRule(ruleData.ProjectId, ruleData.BuildRetentionMinutes, ruleData.EnvironmentNameList, ruleData.EnvironmentIdList, ruleData.MachineNameList, ruleData.MachineIdList);

            AssertCreatedRule(ruleData, result);
            var dbItem = testData.Sut.GetSystemBuildPurgeRule(result.Id);
            AssertRule(result, dbItem);
        }

        [Test]
        public void CreateProjectBuildPurgeRule_MissingProjectID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var ruleData = testData.Fixture.Create<BuildPurgeRule>();

            Assert.Throws<ArgumentNullException>(()=> testData.Sut.CreateProjectBuildPurgeRule(null, ruleData.BuildRetentionMinutes, ruleData.EnvironmentNameList, ruleData.EnvironmentIdList, ruleData.MachineNameList, ruleData.MachineIdList));
        }

        [Test]
        public void UpdateProjectBuildPurgeRule_UpdatesRule()
        {
            var testData = TestData.Create(this, 0, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.ProjectRules[0].Id;
            updatedRuleData.ProjectId = testData.ProjectRules[0].ProjectId;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            var result = testData.Sut.UpdateProjectBuildPurgeRule(updatedRuleData.Id, updatedRuleData.ProjectId, updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList);

            AssertUpdatedRule(testData.ProjectRules[0], updatedRuleData, result);
        }

        [Test]
        public void UpdateProjectBuildPurgeRule_MissingRuleID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.ProjectRules[0].Id;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateProjectBuildPurgeRule(null, updatedRuleData.ProjectId, updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList));
        }

        [Test]
        public void UpdateProjectBuildPurgeRule_BadRuleID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 0, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.ProjectRules[0].Id;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.UpdateProjectBuildPurgeRule(Guid.NewGuid().ToString(), updatedRuleData.ProjectId, updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList));
        }

        [Test]
        public void UpdateProjectBuildPurgeRule_MissingProjectID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.ProjectRules[0].Id;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.UpdateProjectBuildPurgeRule(updatedRuleData.Id, null, updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList));
        }

        [Test]
        public void UpdateProjectBuildPurgeRule_BadProjectID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 0, 1);
            var updatedRuleData = testData.Fixture.Create<BuildPurgeRule>();
            updatedRuleData.Id = testData.ProjectRules[0].Id;
            updatedRuleData.UpdatedByUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(updatedRuleData.UpdatedByUserName);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.UpdateProjectBuildPurgeRule(updatedRuleData.Id, Guid.NewGuid().ToString(), updatedRuleData.BuildRetentionMinutes, updatedRuleData.EnvironmentNameList, updatedRuleData.EnvironmentIdList, updatedRuleData.MachineNameList, updatedRuleData.MachineIdList));
        }

        [Test]
        public void DeleteProjectBuildPurgeRule_DeletesItem()
        {
            var testData = TestData.Create(this, 0, 1);

            var result = testData.Sut.DeleteProjectBuildPurgeRule(testData.ProjectRules[0].Id, testData.ProjectRules[0].ProjectId);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.ProjectRules[0].Id, result.Id);
            Assert.AreEqual(testData.ProjectRules[0].ProjectId, result.ProjectId);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetProjectBuildPurgeRule(testData.ProjectRules[0].Id, testData.ProjectRules[0].ProjectId));
        }

        [Test]
        public void DeleteProjectBuildPurgeRule_MissingRuleID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.DeleteProjectBuildPurgeRule(null, testData.ProjectRules[0].ProjectId));
        }

        [Test]
        public void DeleteProjectBuildPurgeRule_BadRuleID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.DeleteProjectBuildPurgeRule(Guid.NewGuid().ToString(), testData.ProjectRules[0].ProjectId));
        }

        [Test]
        public void DeleteProjectBuildPurgeRule_MissingProjectID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.DeleteProjectBuildPurgeRule(testData.ProjectRules[0].Id, null));
        }

        [Test]
        public void DeleteProjectBuildPurgeRule_BadProjectID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, 0, 1);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.DeleteProjectBuildPurgeRule(testData.ProjectRules[0].Id, Guid.NewGuid().ToString()));
        }
    }
}
