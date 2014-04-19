using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Build
{
    public class SqlServerBuildRepositoryTests : BuildRepositoryTests
    {
        protected override IBuildRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerBuildRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
