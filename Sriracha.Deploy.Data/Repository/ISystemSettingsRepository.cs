using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
    public interface ISystemSettingsRepository
    {
        string GetStringSetting(string key, string defaultValue);
        string SetStringSetting(string key, string value);

        int GetIntSetting(string key, int defaultValue);
        void SetIntSetting(string key, int value);

        List<BaseBuildPurgeRetentionRule> GetBuildPurgeRetentionRuleList(List<BaseBuildPurgeRetentionRule> defaultValue);
        List<BaseBuildPurgeRetentionRule> SetBuildPurgeRetentionRuleList(List<BaseBuildPurgeRetentionRule> value);

        bool AnyActiveSettings();
    }
}
