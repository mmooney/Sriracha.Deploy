using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Deploy
{
    public class SqlServerOfflineDeploymentRepositoryTests : OfflineDeploymentRepositoryTests
    {
        protected override IOfflineDeploymentRepository GetRepository()
        {
            var sqlConnectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerOfflineDeploymentRepository(sqlConnectionInfo, this.UserIdentity.Object);
        }
    }
}
