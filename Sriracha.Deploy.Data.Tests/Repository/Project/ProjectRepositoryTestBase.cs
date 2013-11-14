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
    [TestFixture]
    public abstract class ProjectRepositoryTestBase : RepositoryTestBase<IProjectRepository>
    {
        protected DeployProject CreateTestProject(IProjectRepository sut)
        {
            return sut.CreateProject(this.Fixture.Create<string>("ProjectName"), false);
        }
    }
}
