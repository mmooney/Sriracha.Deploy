using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Deploy
{
    public class SqlServerDeployStateRepositoryTests : DeployStateRepositoryTests
    {
        protected override IDeployStateRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerDeployStateRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
