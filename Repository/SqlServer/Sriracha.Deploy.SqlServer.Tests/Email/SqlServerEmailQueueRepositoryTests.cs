using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Email
{
    public class SqlServerEmailQueueRepositoryTests : EmailQueueRepositoryTests
    {
        protected override IEmailQueueRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerEmailQueueRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
