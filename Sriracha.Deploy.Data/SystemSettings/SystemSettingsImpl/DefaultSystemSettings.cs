using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.SystemSettings.SystemSettingsImpl
{
	public class DefaultSystemSettings : ISystemSettings
	{
        private readonly ISystemSettingsRepository _repository;

        public DefaultSystemSettings(ISystemSettingsRepository repository)
        {
            _repository = DIHelper.VerifyParameter(repository);
        }

		public string FromEmailAddress 
		{
			get { return _repository.GetStringSetting("FromEmailAddress", "sriracha@mmdbsolutions.com"); }
			set { _repository.SetStringSetting("FromEmailAddress", value); }
		}

		public int EmailSenderPollingIntervalSeconds
		{
			get { return _repository.GetIntSetting("EmailSenderPollingIntervalSeconds",60); }
			set { _repository.SetIntSetting("EmailSenderPollingIntervalSeconds", value); }
		}

        public int GCFlushJobIntervalSeconds
        {
            get { return _repository.GetIntSetting("GCFlushJobIntervalSeconds", 60 * 60); }
            set { _repository.SetIntSetting("GCFlushJobIntervalSeconds", value); }
        }

		public int FolderCleanupPollingIntervalSeconds
		{
			get { return _repository.GetIntSetting("FolderCleanupPollingIntervalSeconds", 60*60); }
			set { _repository.SetIntSetting("FolderCleanupPollingIntervalSeconds", value); }
		}
		
        public int PackageOfflineDeploymentPollingIntervalSeconds
        {
            get { return _repository.GetIntSetting("PackageOfflineDeploymentPollingIntervalSeconds", 60); }
            set { _repository.SetIntSetting("PackageOfflineDeploymentPollingIntervalSeconds", value); }
        }
		
        public int RunDeploymentPollingIntervalSeconds
		{
			get { return _repository.GetIntSetting("RunDeploymentPollingIntervalSeconds", 60); }
			set { _repository.SetIntSetting("RunDeploymentPollingIntervalSeconds", value); }
		}

		public string DeployWorkingDirectory
		{
			get { return _repository.GetStringSetting("DeployWorkingDirectory", Path.Combine("C:\\Temp\\Sriracha\\", "WorkingDirectory")); }
			set { _repository.SetStringSetting("DeployWorkingDirectory", value); }
		}

		public string DisplayTimeZoneIdentifier
		{
			get { return _repository.GetStringSetting("DisplayTimeZoneIdentifier", DateTimeHelper.TimeZoneIdentifier_EasternStandardTime); }
			set { _repository.SetStringSetting("DisplayTimeZoneIdentifier",  value); }
		}

		public int LogPurgeJobIntervalSeconds 
        {
			get { return _repository.GetIntSetting("LogPurgeJobIntervalSeconds", 60*60); }
			set { _repository.SetIntSetting("LogPurgeJobIntervalSeconds", value); }
		}

        public int BuildPurgeJobIntervalSeconds
        {
            get { return _repository.GetIntSetting("BuildPurgeJobIntervalSeconds", 60 * 60); }
            set { _repository.GetIntSetting("BuildPurgeJobIntervalSeconds", value); }
        }
        
        public int LogPurgeTraceAgeMinutes
		{
			get { return _repository.GetIntSetting("LogPurgeTraceAgeMinutes", 60); }
			set { _repository.SetIntSetting("LogPurgeTraceAgeMinutes", value); }
		}

		public int LogPurgeDebugAgeMinutes
		{
			get { return _repository.GetIntSetting("LogPurgeDebugAgeMinutes", 60); }
			set { _repository.GetIntSetting("LogPurgeDebugAgeMinutes", value); }
		}

		public int LogPurgeInfoAgeMinutes
		{
			get { return _repository.GetIntSetting("LogPurgeInfoAgeMinutes", 60*24); }
			set { _repository.SetIntSetting("LogPurgeInfoAgeMinutes", value); }
		}

		public int LogPurgeWarnAgeMinutes
		{
			get { return _repository.GetIntSetting("LogPurgeWarnAgeMinutes", 60*24*7); }
			set { _repository.SetIntSetting("LogPurgeWarnAgeMinutes", value); }
		}

		public int LogPurgeErrorAgeMinutes
		{
			get { return _repository.GetIntSetting("LogPurgeErrorAgeMinutes", 60*24*30); }
			set { _repository.SetIntSetting("LogPurgeErrorAgeMinutes", value); }
		}

		public int LogPurgeFatalAgeMinutes
		{
			get { return _repository.GetIntSetting("LogPurgeFatalAgeMinutes", 60*24*30); }
			set { _repository.SetIntSetting("LogPurgeFatalAgeMinutes", value); }
		}

		public int DefaultBuildRetentionMinutes
		{
			get { return _repository.GetIntSetting("DefaultBuildRetentionMinutes", 60*24*7); }
			set { _repository.SetIntSetting("DefaultBuildRetentionMinutes", value); }
		}

        private new List<BaseBuildPurgeRetentionRule> _defaultBuildPurgeRetentionRuleList = new List<BaseBuildPurgeRetentionRule>()
		{
			new DeployHistoryBuildRetentionRule 
			{ 
				BuildRetentionMinutes = 60*24*7,
				EnvironmentNameList = new List<string> { "DEV" }
			},
			new DeployHistoryBuildRetentionRule 
			{
				BuildRetentionMinutes = 60*24*30,
				EnvironmentNameList = new List<string> { "QA", "INT" }
			},
			new DeployHistoryBuildRetentionRule 
			{
				BuildRetentionMinutes = null,
				EnvironmentNameList = new List<string> { "PROD" }
			}
		};

		public List<BaseBuildPurgeRetentionRule> BuildPurgeRetentionRuleList
		{
			get { return _repository.GetBuildPurgeRetentionRuleList(_defaultBuildPurgeRetentionRuleList); }
			set { _repository.SetBuildPurgeRetentionRuleList(value); }
		}

        public List<BaseBuildPurgeRetentionRule> SaveBuildPurgeRetentionRuleList()
        {
            return _repository.SetBuildPurgeRetentionRuleList(this.BuildPurgeRetentionRuleList);
        }

		public int DeploymentFolderCleanupMinutes 
		{ 
			get { return _repository.GetIntSetting("DeploymentFolderCleanupMinutes", 60*24); }
			set { _repository.SetIntSetting("DeploymentFolderCleanupMinutes", value); }
		}

        public string OfflineExeDirectory
        {
            get { return _repository.GetStringSetting("_offlineExeDirectory", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "offlineExe")); }
            set { _repository.SetStringSetting("OfflineExeDirectory", value); }
        }

        public bool AllowSelfRegistration
        {
            get { return _repository.GetBoolSetting("AllowSelfRegistration", true); }
            set { _repository.SetBoolSetting("AllowSelfRegistration", value); }
        }

        public EnumPermissionAccess DefaultAccess
        {
            get { return _repository.GetEnumSetting<EnumPermissionAccess>("EnumPermissionAccess", EnumPermissionAccess.Grant); }
            set { _repository.SetEnumSetting("EnumPermissionAccess", value); }
        }

        public bool IsInitialized()
        {
            return _repository.AnyActiveSettings();
        }


    }
}
