using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;

namespace Sriracha.Deploy.Data.Impl
{
	public class DefaultSystemSettings : ISystemSettings
	{
		private string _fromEmailAddress;
		public string FromEmailAddress 
		{
			get { return StringHelper.IsNullOrEmpty(_fromEmailAddress, "sriracha@mmdbsolutions.com"); }
			set { _fromEmailAddress = value; }
		}

		private int? _emailSenderPollingIntervalSeconds;
		public int EmailSenderPollingIntervalSeconds
		{
			get { return _emailSenderPollingIntervalSeconds.GetValueOrDefault(60); }
			set { _emailSenderPollingIntervalSeconds = value; }
		}

		private int? _runDeploymentPollingIntervalSeconds;
		public int RunDeploymentPollingIntervalSeconds
		{
			get { return _runDeploymentPollingIntervalSeconds.GetValueOrDefault(60); }
			set { _runDeploymentPollingIntervalSeconds = value; }
		}

		private string _deployWorkingDirectory;
		public string DeployWorkingDirectory
		{
			get 
			{
				if(!string.IsNullOrEmpty(_deployWorkingDirectory))
				{
					return _deployWorkingDirectory;
				}
				else 
				{
					return Path.Combine("C:\\Temp\\Sriracha\\", "WorkingDirectory"); 
				}
			}
			set 
			{
				_deployWorkingDirectory = value;
			}
		}

		private string _displayTimeZoneIdentifier;
		public string DisplayTimeZoneIdentifier
		{
			get 
			{
				return StringHelper.IsNullOrEmpty(_displayTimeZoneIdentifier, DateTimeHelper.TimeZoneIdentifier_EasternStandardTime);
			}
			set 
			{
				_displayTimeZoneIdentifier = value;
			}
		}

		private int? _logPurgeJobIntervalSeconds;
		public int LogPurgeJobIntervalSeconds
		{
			get { return _logPurgeJobIntervalSeconds.GetValueOrDefault(60*60); }
			set { _logPurgeJobIntervalSeconds = value; }
		}

		private int? _logPurgeTraceAgeMinutes;
		public int? LogPurgeTraceAgeMinutes
		{
			get { return _logPurgeTraceAgeMinutes.GetValueOrDefault(60); }
			set { _logPurgeTraceAgeMinutes = value; }
		}

		private int? _logPurgeDebugAgeMinutes;
		public int? LogPurgeDebugAgeMinutes
		{
			get { return _logPurgeDebugAgeMinutes.GetValueOrDefault(60); }
			set { _logPurgeDebugAgeMinutes = value; }
		}

		private int? _logPurgeInfoAgeMinutes;
		public int? LogPurgeInfoAgeMinutes
		{
			get { return _logPurgeInfoAgeMinutes.GetValueOrDefault(60*24); }
			set { _logPurgeInfoAgeMinutes = value; }
		}

		private int? _logPurgeWarnAgeMinutes;
		public int? LogPurgeWarnAgeMinutes
		{
			get { return _logPurgeWarnAgeMinutes.GetValueOrDefault(60*24*7); }
			set { _logPurgeWarnAgeMinutes = value; }
		}

		private int? _logPurgeErrorAgeMinutes;
		public int? LogPurgeErrorAgeMinutes
		{
			get { return _logPurgeErrorAgeMinutes.GetValueOrDefault(60*24*30); }
			set { _logPurgeErrorAgeMinutes = value; }
		}

		private int? _logPurgeFatalAgeMinutes;
		public int? LogPurgeFatalAgeMinutes
		{
			get { return _logPurgeFatalAgeMinutes.GetValueOrDefault(60*24*30); }
			set { _logPurgeFatalAgeMinutes = value; }
		}

		private int? _buildPurgeJobIntervalSeconds;
		public int BuildPurgeJobIntervalSeconds
		{
			get { return _buildPurgeJobIntervalSeconds.GetValueOrDefault(60*60); }
			set { _buildPurgeJobIntervalSeconds = value; }
		}

		private int? _defaultBuildRetentionMinutes;
		public int? DefaultBuildRetentionMinutes
		{
			get { return _defaultBuildRetentionMinutes.GetValueOrDefault(60*24); }
			set { _defaultBuildRetentionMinutes = value; }
		}

		private List<BaseBuildPurgeRetentionRule> _buildPurgeRetentionRuleList;
		public List<BaseBuildPurgeRetentionRule> BuildPurgeRetentionRuleList
		{
			get
			{
				if (_buildPurgeRetentionRuleList == null)
				{
					_buildPurgeRetentionRuleList = new List<BaseBuildPurgeRetentionRule>()
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
				}
				return _buildPurgeRetentionRuleList;
			}
			set
			{
				_buildPurgeRetentionRuleList = value;
			}
		}

		private int? _gcFlushJobIntervalSeconds;

		public int GCFlushJobIntervalSeconds
		{
			get { return _gcFlushJobIntervalSeconds.GetValueOrDefault(60*60); }
			set { _gcFlushJobIntervalSeconds = value; }
		}

		private int? _deploymentFolderCleanupMinutes;
		public int DeploymentFolderCleanupMinutes 
		{ 
			get { return _deploymentFolderCleanupMinutes.GetValueOrDefault(60*24); }
			set { _deploymentFolderCleanupMinutes = value; }
		}

        private string _offlineExeDirectory;
        public string OfflineExeDirectory
        {
            get { return StringHelper.IsNullOrEmpty(_offlineExeDirectory, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "offlineExe")); }
            set { _offlineExeDirectory = value; }
        }
    }
}
