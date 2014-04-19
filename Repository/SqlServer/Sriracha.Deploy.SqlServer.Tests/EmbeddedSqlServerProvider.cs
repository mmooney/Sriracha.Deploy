using MMDB.Shared;
using Sriracha.Deploy.Data.Utility.UtilityImpl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sriracha.Deploy.SqlServer.Tests
{
    public static class EmbeddedSqlServerProvider
    {
        public static volatile bool _isLoaded = false;
        public static object _locker = new object();
         
        public static ISqlConnectionInfo GetSqlConnectionInfo()
        {
            lock(_locker)
            {
                if(!_isLoaded)
                {
                    string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string roundhousePath = Path.Combine(directory, "rh.exe");
                    var connectionString = ConfigurationManager.ConnectionStrings["SrirachaSql"];
                    if(connectionString == null || string.IsNullOrEmpty(connectionString.ConnectionString))
                    {
                        throw new Exception("Missing connection string SrirachaSql");
                    }
                    string scriptRelativePath = AppSettingsHelper.GetRequiredSetting("ScriptRelativePath");
                    string scriptDirectory;
                    if(NCrunch.Framework.NCrunchEnvironment.NCrunchIsResident())
                    {
                        scriptDirectory = Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath()), scriptRelativePath);
                    }
                    else 
                    {
                        scriptDirectory = Path.GetFullPath(scriptRelativePath);
                        if (!Directory.Exists(scriptDirectory))
                        {
                            scriptDirectory = Path.GetFullPath(Path.Combine("..", scriptRelativePath));
                        }
                        if (!Directory.Exists(scriptDirectory))
                        {
                            scriptDirectory = Path.GetFullPath(Path.Combine("..", "..", scriptRelativePath));
                        }
                    }
                    if(!Directory.Exists(scriptDirectory))
                    {
                        throw new Exception("Script directory not found: " + scriptDirectory);
                    }
                    string parameters = string.Format("--connstring \"{0}\" --files=\"{1}\" --silent", connectionString.ConnectionString, scriptDirectory);
                    var processRunner = new ProcessRunner();
                    var outputWriter = new StringWriter();
                    var errorWriter = new StringWriter();
                    int exitCode = processRunner.Run(roundhousePath, parameters, outputWriter, errorWriter);
                    _isLoaded = true;
                }
                return new SqlConnectionInfo();
            }
        }
    }
}
