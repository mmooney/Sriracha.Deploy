using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Project
{
    public class SqlServerPermissionRepositoryTests : PermissionRepositoryTests
    {
        protected override IPermissionRepository GetRepository()
        {
            var sqlConnectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerPermissionRepository(sqlConnectionInfo, this.UserIdentity.Object);
        }
    }
}
