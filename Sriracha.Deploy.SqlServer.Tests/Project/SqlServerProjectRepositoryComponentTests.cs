using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Project
{
    [TestFixture]
    public class SqlServerProjectRepositoryComponentTests : ProjectRepositoryComponentTests
    {
        protected override IProjectRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerProjectRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
