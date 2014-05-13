using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Cleanup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Cleanup
{
    public class SqlServerCleanupRepositoryTests : CleanupRepositoryTests
    {
        protected override ICleanupRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerCleanupRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
