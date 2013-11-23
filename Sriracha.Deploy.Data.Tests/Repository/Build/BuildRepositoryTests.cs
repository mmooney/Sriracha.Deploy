using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository.Build
{
    public abstract class BuildRepositoryTests : RepositoryTestBase<IBuildRepository>
    {
        [Test]
        public void CreateBuild_CreatesBuild()
        {
        }
    }
}
