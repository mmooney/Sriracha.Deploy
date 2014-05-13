using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Credentials
{
    public class SqlServerCredentialsRepositoryTests : CredentialsRepositoryTests
    {
        protected override ICredentialsRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerCredentialsRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
