using MMDB.ConnectionSettings;
using Sriracha.Deploy.Data.Tests.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests
{
    public class SqlServerConnectionSettingRepositoryTests : ConnectionSettingRepositoryTests
    {
        protected override IConnectionSettingRepository GetRepository()
        {
            var sqlConnectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerConnectionSettingRepository(sqlConnectionInfo);
        }
    }
}
