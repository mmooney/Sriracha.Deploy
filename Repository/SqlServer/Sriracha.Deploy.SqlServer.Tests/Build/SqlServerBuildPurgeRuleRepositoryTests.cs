using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Build
{
    public class SqlServerBuildPurgeRuleRepositoryTests : BuildPurgeRuleRepositoryTests
    {
        protected override IBuildPurgeRuleRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerBuildPurgeRuleRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
