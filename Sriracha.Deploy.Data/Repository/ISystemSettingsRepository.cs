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


        bool GetBoolSetting(string key, bool defaultValue);
        void SetBoolSetting(string key, bool value);

        T GetEnumSetting<T>(string key, T defaultValue) where T:struct, IConvertible;
        void SetEnumSetting<T>(string key, T value) where T : struct, IConvertible;

        bool AnyActiveSettings();
        void InactivateActiveSettings();
    }
}
