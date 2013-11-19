using NUnit.Framework;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;

namespace Sriracha.Deploy.Data.Tests.Repository.Project
{
    public abstract class ProjectRepositoryTestBase : RepositoryTestBase<IProjectRepository>
    {
        protected DeployProject CreateTestProject(IProjectRepository sut)
        {
            return sut.CreateProject(this.Fixture.Create<string>("ProjectName"), false);
        }

        protected void AssertProject(DeployProject expected, DeployProject actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsNotNullOrEmpty(actual.Id);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectName, actual.ProjectName);
            Assert.AreEqual(expected.UsesSharedComponentConfiguration, actual.UsesSharedComponentConfiguration);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }
    }
}
