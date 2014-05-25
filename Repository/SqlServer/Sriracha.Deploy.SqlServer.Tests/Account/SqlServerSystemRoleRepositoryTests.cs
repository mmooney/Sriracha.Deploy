using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Account
{
    public class SqlServerSystemRoleRepositoryTests : SystemRoleRepositoryTests
    {
        protected override ISystemRoleRepository GetRepository()
        {
            var sqlConnectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerSystemRoleRepository(sqlConnectionInfo, this.UserIdentity.Object);
        }
    }
}
