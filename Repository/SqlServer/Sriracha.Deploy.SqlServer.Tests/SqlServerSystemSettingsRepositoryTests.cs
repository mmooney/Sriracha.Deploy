using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests
{
    public class SqlServerSystemSettingsRepositoryTests : SystemSettingsRepositoryTests
    {
        protected override ISystemSettingsRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return  new SqlServerSystemSettingsRepository(connectionInfo);
        }
    }
}
