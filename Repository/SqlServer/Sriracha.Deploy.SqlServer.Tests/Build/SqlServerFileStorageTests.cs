using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Tests.Repository.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests.Build
{
    public class SqlServerFileStorageTests : FileStorageTests
    {
        protected override IFileStorage GetRepository()
        {
            var connectionInfo = EmbeddedSqlServerProvider.GetSqlConnectionInfo();
            return new SqlServerFileStorage(connectionInfo, this.UserIdentity.Object);
        }
    }
}
