using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Build
{
    public class SqlServerFileRepositoryTests : FileRepositoryTests
    {
        protected override IFileRepository GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerFileRepository(connectionInfo, this.UserIdentity.Object, this.FileStorage.Object);
        }
    }
}
