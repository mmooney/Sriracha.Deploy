using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.RoundhousE.DeployDatabase
{
    public enum EnumRoundhouseDatabaseType
    {
        Unknown,
        SqlServer,
        Oracle,
        Postgres,
        //Access,
        //MySql,
        SqlLite
    }

    public class DeployRoundhouseDatabaseTaskOptions
    {
        public EnumRoundhouseDatabaseType DatabaseType { get; set; }
        public string RespositoryPath { get; set; }

        public string EnvironmentName { get; set; }

        public string SqlFilesDirectory { get; set; }
    }
}
