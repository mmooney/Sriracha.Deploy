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
using dropkick.Configuration;
using dropkick.Tasks.RoundhousE;
using dropkick.Wmi;

namespace Sriracha.Deploy.SelfDeploy
{
    public class DeploymentSettings : DropkickConfiguration
    {
		//General Config
		public string RavenDBConnectionString { get; set; }
		public string EmailConnectionString { get; set; }
		public string SiteUrl { get; set; }
		public string EncryptionKey { get; set; }

		//Web
		public string SourceWebsitePath { get; set; }
		public string TargetWebsitePath{ get; set; }
		public string WebUserName { get; set; }
		public string VirtualDirectorySite { get; set; }
		public string VirtualDirectoryName { get; set; }
		public string ApplicationPoolName { get; set; }
		public string WebsiteAuthenticationMode { get; set; }
		//#region Properties

		//Service 
		public ServiceStartMode ServiceStartMode { get; set; }
		public string ServiceUserName { get; set; }
		public string ServiceUserPassword { get; set; }
		public string ServiceSourcePath { get; set; }
		public string ServiceTargetPath { get; set; }
		public string ServiceName { get; set; }
		public bool AutoStartService { get; set; }
		public string ServiceExeName { get; set; }

		//Command Line 
		public string SourceCommandLinePath { get; set; }
		public string TargetCommandLinePath { get; set; }
		public string CommandLineExeName { get; set; }
	}
}