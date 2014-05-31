using Microsoft.CSharp;
using MMDB.Shared;
using Sriracha.Deploy.Data.Credentials;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Sriracha.Deploy.Data.Dropkick
{
    public class DropkickRunnerContext<TaskDefinitionType, TaskOptionsType> : IDisposable
            where TaskDefinitionType : IDeployTaskDefinition
    {
        private readonly TaskExecutionContext<TaskDefinitionType, TaskOptionsType> _taskExecutionContext;
        private readonly IProcessRunner _processRunner;
        private readonly ICredentialsManager _credentialsManager;
        private readonly IImpersonator _impersonator;
        private readonly string _dropkickDirectory;
        //private bool _disposed = false;

        public DropkickRunnerContext(IProcessRunner processRunner, ICredentialsManager credentialsManager, IImpersonator impersonator, TaskExecutionContext<TaskDefinitionType, TaskOptionsType> taskExecutionContext, string dropkickDirectory)
        {
            _processRunner = processRunner;
            _credentialsManager = credentialsManager;
            _impersonator = impersonator;
            _taskExecutionContext = taskExecutionContext;
            _dropkickDirectory = dropkickDirectory;
        }

        public void Dispose()
        {
            //if(!this._disposed)
            //{
            //    if(Directory.Exists(_dropkickDirectory))
            //    {
            //        try 
            //        {
            //            Directory.Delete(_dropkickDirectory);
            //        }
            //    }
            //}
        }

        public void Run<DeploymentType>(object settings, Dictionary<string, string> serverMaps)
        {
            string settingsDirectory = Path.Combine(_dropkickDirectory, "settings");
            if(!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }
            string environmentName = _taskExecutionContext.Machine.EnvironmentName.Replace(" ","");
            string settingsFilePath = Path.Combine(settingsDirectory, environmentName + ".js");
            File.WriteAllText(settingsFilePath, settings.ToJson());

            string serverMapFilePath = Path.Combine(settingsDirectory, environmentName + ".servermaps");
            File.WriteAllText(serverMapFilePath, serverMaps.ToJson());

 
            string deploymentFilePath = typeof(DeploymentType).Assembly.Location;

            string dropkickExePath = string.Format("{0}\\dk.exe", _dropkickDirectory);
            #pragma warning Need to wrap these parameters in quotes, but need a fix in dropkick first.
            string exeParameters = string.Format("execute /deployment:{0} /environment:{1} /settings:{2} --silent", deploymentFilePath, environmentName, settingsDirectory);

            using (var standardOutputWriter = new StringWriter())
            using (var errorOutputWriter = new StringWriter())
            {
                int exeResult;
                if (string.IsNullOrEmpty(_taskExecutionContext.EnvironmentComponent.DeployCredentialsId) || !AppSettingsHelper.GetBoolSetting("AllowImpersonation", true))
                {  
                    exeResult = _processRunner.Run(dropkickExePath, exeParameters, standardOutputWriter, errorOutputWriter, Path.GetDirectoryName(deploymentFilePath));
                }
                else
                {
                    var credentials = _credentialsManager.GetCredentials(_taskExecutionContext.EnvironmentComponent.DeployCredentialsId);
                    using (var impersonation = _impersonator.BeginImpersonation(credentials.Id))
                    {
                        _taskExecutionContext.Info("Starting process as {0} impersonating {1}", WindowsIdentity.GetCurrent().Name, credentials.DisplayValue);
                        using (var password = _credentialsManager.DecryptPasswordSecure(credentials))
                        {
                            string fullExePath = Path.GetFullPath(dropkickExePath);
                            _taskExecutionContext.Info("For Options.ExecutablePath {0}, using {1}", dropkickExePath, fullExePath);
                            //result = _processRunner.RunAsUser(exePath, formattedArgs, standardOutputWriter, errorOutputWriter, credentials.Domain, credentials.UserName, password);
                            exeResult = _processRunner.RunAsToken(fullExePath, exeParameters, standardOutputWriter, errorOutputWriter, impersonation.TokenHandle);
                        }
                    }
                }
                string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
                string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
                if (exeResult == 0)
                {
                    if (!string.IsNullOrWhiteSpace(standardOutput))
                    {
                        _taskExecutionContext.Info(standardOutput);
                    }
                    if (!string.IsNullOrWhiteSpace(errorOutput))
                    {
                        _taskExecutionContext.Error(errorOutput);
                        throw new Exception("DropKickRunner Failed: " + errorOutput);
                    }
                }
                else
                {
                    string errorText;
                    if (!string.IsNullOrWhiteSpace(errorOutput))
                    {
                        errorText = errorOutput;
                    }
                    else if (!string.IsNullOrWhiteSpace(standardOutput))
                    {
                        errorText = standardOutput;
                    }
                    else
                    {
                        errorText = "Error Code " + exeResult.ToString();
                    }
                    _taskExecutionContext.Error(errorText);
                    throw new Exception("DropkickRunner Task Failed: " + errorText);
                }
            }

        }
    }
}
