//ReSharper disable ConvertToLambdaExpression
// ==============================================================================
// 
// ACuriousMind and FerventCoder Copyright © 2011 - Released under the Apache 2.0 License
// 
// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
// ==============================================================================
using System;
using System.IO;
using dropkick.Configuration.Dsl;
using dropkick.Configuration.Dsl.Files;
using dropkick.Configuration.Dsl.Iis;
using dropkick.Configuration.Dsl.RoundhousE;
using dropkick.Configuration.Dsl.Security;
using dropkick.Configuration.Dsl.WinService;
using dropkick.Configuration.Dsl.Xml;
using dropkick.Configuration.Dsl.Notes;
using dropkick.Wmi;
using dropkick.Configuration.Dsl.Authentication;

namespace Sriracha.Deploy.SelfDeploy
{
	public class TheDeployment : Deployment<TheDeployment, DeploymentSettings>
	{
		#region Constructors

		public TheDeployment()
		{
			Define(settings =>
			{
				//	DeploymentStepsFor(Db,
				//					   s =>
				//					   {
				//						   s.RoundhousE()
				//							   .ForEnvironment(settings.Environment)
				//							   .OnDatabase(settings.DbName)
				//							   .WithScriptsFolder(settings.DbSqlFilesPath)
				//							   .WithDatabaseRecoveryMode(settings.DbRecoveryMode)
				//							   .WithRestorePath(settings.DbRestorePath)
				//							   .WithRepositoryPath("__REPLACE_ME__")
				//							   .WithVersionFile("_BuildInfo.xml")
				//							   .WithRoundhousEMode(settings.RoundhousEMode)
				//							   ;
				//					   });

				DeploymentStepsFor(Web,
									s =>
									{
										ValidateSettings(settings);

                                        if (!string.IsNullOrEmpty(settings.TargetMachineUserName) && !string.IsNullOrEmpty(settings.TargetMachinePassword))
                                        {
                                            s.OpenFolderShareWithAuthentication(@"{{TargetWebsitePath}}", settings.TargetMachineUserName, settings.TargetMachinePassword);
                                        }

										s.CopyDirectory(settings.SourceWebsitePath).To(@"{{TargetWebsitePath}}").DeleteDestinationBeforeDeploying();

										ApplySettings(s, settings, @"{{TargetWebsitePath}}\web.config");
										s.Security(securityOptions =>
										{
											securityOptions.ForPath(settings.TargetWebsitePath, fileSecurityConfig => fileSecurityConfig.GrantRead(settings.WebUserName));
											////securityOptions.ForPath(Path.Combine(settings.WebsitePath, "logs"), fs => fs.GrantReadWrite(settings.WebUserName));
											//securityOptions.ForPath(@"~\C$\Windows\Microsoft.NET\Framework\v4.0.30319\Temporary ASP.NET Files", fs => fs.GrantReadWrite(settings.WebUserName));
											//if (Directory.Exists(@"~\C$\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files"))
											//{
											//	securityOptions.ForPath(@"~\C$\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files", fs => fs.GrantReadWrite(settings.WebUserName));
											//}
										});
									});

                DeploymentStepsFor(VirtualDirectory,
                                    s =>
                                    {
                                        string appPoolName = settings.ApplicationPoolName;
                                        if (string.IsNullOrWhiteSpace(appPoolName))
                                        {
                                            appPoolName = settings.VirtualDirectorySite;
                                        }
                                        if (!string.IsNullOrEmpty(settings.TargetMachineUserName) && !string.IsNullOrEmpty(settings.TargetMachinePassword))
                                        {
                                            s.WithAuthentication(settings.TargetMachineUserName, settings.TargetMachineUserName);
                                        }
                                        var iis = s.Iis7Site(settings.VirtualDirectorySite)
                                         .VirtualDirectory(settings.VirtualDirectoryName)
                                         .SetAppPoolTo(appPoolName, pool =>
                                                         {
                                                             pool.SetRuntimeToV4();
                                                             //pool.UseClassicPipeline();
                                                             //pool.Enable32BitAppOnWin64();
                                                         }).SetPathTo(@"{{TargetWebsitePath}}");
                                    });

				DeploymentStepsFor(Host,
									s =>
									{
										ValidateSettings(settings);

                                        if(string.IsNullOrEmpty(settings.SourceOfflineExePath))
                                        {
                                            throw new Exception("Missing setting SourceOfflineExePath");
                                        }
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
                                        s.CopyDirectory(settings.SourceOfflineExePath).To(@"{{ServiceTargetPath}}\offlineExe").DeleteDestinationBeforeDeploying();

										ApplySettings(s, settings, @"{{ServiceTargetPath}}\{{ServiceExeName}}.config");

                                        //TODO: Figure out how to do this remotely
                                        //s.Security(o =>
                                        //{
                                        //    o.LocalPolicy(lp =>
                                        //    {
                                        //        lp.LogOnAsService(settings.ServiceUserName);
                                        //        lp.LogOnAsBatch(settings.ServiceUserName);
                                        //    });

                                        //    o.ForPath(settings.ServiceTargetPath, fs => fs.GrantRead(settings.ServiceUserName));
                                        //    //o.ForPath(Path.Combine(settings.ServiceTargetPath, "logs"), fs => fs.GrantReadWrite(settings.ServiceUserName));
                                        //});

										s.XmlPoke(@"{{ServiceTargetPath}}\{{ServiceExeName}}.config")
														.Set("/configuration/connectionStrings/add[@name='RavenDB']/@connectionString", settings.RavenDBConnectionString);

										serviceOptions.Delete();
										serviceOptions.Create()
											.WithCredentials(settings.ServiceUserName, settings.ServiceUserPassword)
											//.WithDisplayName("__REPLACE_ME__ ({{Environment}})")
											.WithServicePath(@"{{ServiceTargetPath}}\{{ServiceExeName}}")
											.WithDisplayName(serviceName)
											.WithStartMode(settings.ServiceStartMode)
											//.AddDependency("MSMQ")
											;

										if (settings.ServiceStartMode != ServiceStartMode.Disabled && settings.ServiceStartMode != ServiceStartMode.Manual)
										{
											if (settings.AutoStartService)
											{
												serviceOptions.Start();
											}
										}
									});
					DeploymentStepsFor(CommandLine,
									s =>
									{
										this.ValidateSettings(settings);
                                        if(!string.IsNullOrEmpty(settings.TargetMachineUserName) && !string.IsNullOrEmpty(settings.TargetWebsitePath))
                                        {
                                            s.OpenFolderShareWithAuthentication(@"{{TargetCommandLinePath}}", settings.TargetMachineUserName, settings.TargetMachinePassword);
                                        }
    									s.CreateEmptyFolder(@"{{TargetCommandLinePath}}");
										s.CopyDirectory(settings.SourceCommandLinePath).To(@"{{TargetCommandLinePath}}").ClearDestinationBeforeDeploying();

										this.ApplySettings(s, settings, @"{{TargetCommandLinePath}}\{{CommandLineExeName}}.config");
								   });
			});
		}

		private void ApplySettings(ProtoServer s, DeploymentSettings settings, string configPath)
		{
			s.XmlPoke(configPath).Set("/configuration/connectionStrings/add[@name='Email']/@connectionString", settings.EmailConnectionString);
			s.XmlPoke(configPath).Set("/configuration/appSettings/add[@key='SiteUrl']/@value", settings.SiteUrl);
			s.XmlPoke(configPath).Set("/configuration/appSettings/add[@key='EncryptionKey']/@value", settings.EncryptionKey);
            s.XmlPoke(configPath).Set("/configuration/appSettings/add[@key='RepositoryAssemblyName']/@value", settings.RepositoryAssemblyName);
            if (!string.IsNullOrWhiteSpace(settings.RavenDBConnectionString))
            {
                s.XmlPoke(configPath)
                    .Set("/configuration/connectionStrings/add[@name='RavenDB']/@connectionString", settings.RavenDBConnectionString);
            }
            if (!string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                s.XmlPoke(configPath)
                    .Set("/configuration/connectionStrings/add[@name='SrirachaSql']/@connectionString", settings.ConnectionString);
            }

			if (!string.IsNullOrEmpty(settings.WebsiteAuthenticationMode))
			{
				s.XmlPoke(configPath)
							 .Set("/configuration/system.web/authentication/@mode", settings.WebsiteAuthenticationMode);
			}

		}

		private void ValidateSettings(DeploymentSettings settings)
		{
            if(!string.IsNullOrEmpty(settings.TargetMachineUserName) && string.IsNullOrEmpty(settings.TargetMachinePassword))
            {
                throw new Exception("Cannot have TargetMachineUserName without TargetMachinePassword");
            }
            if (string.IsNullOrEmpty(settings.TargetMachineUserName) && !string.IsNullOrEmpty(settings.TargetMachinePassword))
            {
                throw new Exception("Cannot have TargetMachinePassword without TargetMachineUserName");
            }
            if (string.IsNullOrEmpty(settings.EmailConnectionString))
			{
				throw new Exception("Missing EmailConnectionString");
			}
			if (string.IsNullOrEmpty(settings.SiteUrl))
			{
				throw new Exception("Missing SiteUrl");
			}
			if(string.IsNullOrEmpty(settings.EncryptionKey))
			{
				throw new Exception("Missing EncryptionKey");
			}
            if(string.IsNullOrEmpty(settings.RepositoryAssemblyName))
            {
                throw new Exception("Missing RepositoryAssemblyName");
            }
		}

		#endregion

		#region Properties

		//order is important
		//public static Role Db { get; set; }
		public static Role Web { get; set; }
		public static Role VirtualDirectory { get; set; }
		public static Role Host { get; set; }
		public static Role CommandLine { get; set; }

		#endregion
	}
}