using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Account
{
    public class SqlServerMembershipRepositoryTests : MembershipRepositoryTests
    {
        protected override IMembershipRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerMembershipRepository(connectionInfo, this.UserIdentity.Object);
        }
    }
}
