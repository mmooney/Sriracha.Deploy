using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests
{
    public class SqlServerRazorTemplateRepositoryTests : RazorTemplateRepositoryTests
    {
        protected override IRazorTemplateRepository GetRepository()
        {
            var sqlConnectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerRazorTemplateRepository(sqlConnectionInfo, this.UserIdentity.Object);
        }
    }
}
