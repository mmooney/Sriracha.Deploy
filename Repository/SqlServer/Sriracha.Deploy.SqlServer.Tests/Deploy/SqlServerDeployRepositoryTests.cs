using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Deploy
{
    public class SqlServerDeployRepositoryTests : DeployRepositoryTests
    {
        protected override IDeployRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerDeployRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
