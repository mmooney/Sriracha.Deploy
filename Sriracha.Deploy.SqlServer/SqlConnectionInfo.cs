using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlConnectionInfo : ISqlConnectionInfo
    {

        public PetaPoco.Database GetDB()
        {
            string connectionStringKey = "SrirachaSql";
            return new PetaPoco.Database(connectionStringKey);
        }
    }
}
