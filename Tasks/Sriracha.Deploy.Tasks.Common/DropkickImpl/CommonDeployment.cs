using dropkick.Configuration.Dsl;
using dropkick.Configuration.Dsl.Files;
using dropkick.Configuration.Dsl.WinService;
using dropkick.Configuration.Dsl.Authentication;
using dropkick.Configuration.Dsl.Iis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Tasks.Common.DropkickImpl
{
    internal class CommonDeployment : Deployment<CommonDeployment, CommonDeploymentSettings>
    {
        public static Role Service { get; set; }
        public static Role Website { get; set; }

        public CommonDeployment()
		{
			Define(settings =>
			{
				DeploymentStepsFor(Service,
									s =>
									{
                                        //ValidateSettings(settings);

                                        //if(string.IsNullOrEmpty(settings.SourceOfflineExePath))
                                        //{
                                        //    throw new Exception("Missing setting SourceOfflineExePath");
                                        //}
                                        if (!string.IsNullOrEmpty(settings.TargetMachineUserName) && !string.IsNullOrEmpty(settings.TargetMachinePassword))
                                        {
                                            s.WithAuthentication(settings.TargetMachineUserName, settings.TargetMachinePassword);

                                            s.OpenFolderShareWithAuthentication(@"{{ServiceTargetPath}}", settings.TargetMachineUserName, settings.TargetMachinePassword);
                                        }

                                        var serviceName = settings.ServiceName;
                                        var serviceOptions = s.WinService(serviceName);

                                        serviceOptions.Stop();
                                        s.CreateEmptyFolder(@"{{ServiceTargetPath}}");
                                        s.CopyDirectory(settings.ServiceSourcePath).To(@"{{ServiceTargetPath}}").DeleteDestinationBeforeDeploying();

                                        s.CopyFile(settings.ServiceSourceExeConfigPath).ToDirectory(@"{{ServiceTargetPath}}").RenameTo("{{ServiceExeName}}.config");

                                        //ApplySettings(s, settings, @"{{ServiceTargetPath}}\{{ServiceExeName}}.config");

                                        ////TODO: Figure out how to do this remotely
                                        ////s.Security(o =>
                                        ////{
                                        ////    o.LocalPolicy(lp =>
                                        ////    {
                                        ////        lp.LogOnAsService(settings.ServiceUserName);
                                        ////        lp.LogOnAsBatch(settings.ServiceUserName);
                                        ////    });

                                        ////    o.ForPath(settings.ServiceTargetPath, fs => fs.GrantRead(settings.ServiceUserName));
                                        ////    //o.ForPath(Path.Combine(settings.ServiceTargetPath, "logs"), fs => fs.GrantReadWrite(settings.ServiceUserName));
                                        ////});

                                        //s.XmlPoke(@"{{ServiceTargetPath}}\{{ServiceExeName}}.config")
                                        //                .Set("/configuration/connectionStrings/add[@name='RavenDB']/@connectionString", settings.RavenDBConnectionString);

                                        serviceOptions.Delete();
                                        var service = serviceOptions.Create()
                                            .WithCredentials(settings.ServiceUserName, StringHelper.IsNull(settings.ServiceUserPassword, string.Empty))
                                            //.WithDisplayName("__REPLACE_ME__ ({{Environment}})")
                                            .WithServicePath(@"{{ServiceTargetPath}}\{{ServiceExeName}}")
                                            .WithDisplayName(serviceName)
                                            .WithStartMode(settings.ServiceStartMode)
                                            //.AddDependency("MSMQ")
                                            ;
                                        if(settings.DependencyList != null)
                                        {
                                            foreach(var dependency in settings.DependencyList)
                                            {
                                                service = service.AddDependency(dependency);
                                            }
                                        }

                                        if (settings.StartImmediately)
                                        {
                                            serviceOptions.Start();
                                        }
									});
			});
            Define(settings =>
            {
                DeploymentStepsFor(Website,
                                    s =>
                                    {
                                        if (!string.IsNullOrEmpty(settings.TargetMachineUserName) && !string.IsNullOrEmpty(settings.TargetMachinePassword))
                                        {
                                            s.WithAuthentication(settings.TargetMachineUserName, settings.TargetMachinePassword);

                                            s.OpenFolderShareWithAuthentication(@"{{TargetWebsitePath}}", settings.TargetMachineUserName, settings.TargetMachinePassword);
                                        }

                                        string appPoolName = settings.ApplicationPoolName;
                                        if (string.IsNullOrWhiteSpace(appPoolName))
                                        {
                                            appPoolName = settings.SiteName;
                                        }
                                        var iis = s.Iis7Site(settings.SiteName, @"{{TargetWebsitePath}}", default(int))
                                            .VirtualDirectory(StringHelper.IsNullOrEmpty(settings.VirtualDirectoryName,"/"))
                                            .SetAppPoolTo(appPoolName, pool =>
                                            {
                                                pool.SetRuntimeToV4();
                                                //pool.UseClassicPipeline();
                                                //pool.Enable32BitAppOnWin64();
                                            }).SetPathTo(@"{{TargetWebsitePath}}");
                                    });
            });
        }

        public Dictionary<string, string> GetDefaultServerMap()
        {
            var returnValue = new Dictionary<string, string>();
            returnValue.Add("Service", string.Empty);
            returnValue.Add("Website", string.Empty);
            return returnValue;
        }
    }
}
