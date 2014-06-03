using MMDB.Shared;
using ServiceStack.Net30.Collections.Concurrent;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineSystemSettingsRepository : ISystemSettingsRepository
    {
        private ConcurrentDictionary<string, string> _values;

        public OfflineSystemSettingsRepository()
        {
            _values = new ConcurrentDictionary<string,string>();
        }

        public string GetStringSetting(string key, string defaultValue)
        {
            string returnValue = null;
            if(!_values.TryGetValue(key, out returnValue))
            {
                return defaultValue;
            }
            return returnValue;
        }

        public string SetStringSetting(string key, string value)
        {
            return _values[key] = value;
        }

        public int GetIntSetting(string key, int defaultValue)
        {
            var stringValue = GetStringSetting(key, defaultValue.ToString());
            int returnValue;
            if (!int.TryParse(stringValue, out returnValue))
            {
                throw new FormatException(string.Format("Unable to parse {0} value \"{1}\" into an integer", key, stringValue));
            }
            return returnValue;
        }

        public void SetIntSetting(string key, int value)
        {
            this.SetStringSetting(key, value.ToString());
        }

        public bool GetBoolSetting(string key, bool defaultValue)
        {
            var stringValue = GetStringSetting(key, defaultValue.ToString());
            bool returnValue;
            if (!bool.TryParse(stringValue, out returnValue))
            {
                throw new FormatException(string.Format("Unable to parse {0} value \"{1}\" into an boolean", key, stringValue));
            }
            return returnValue;
        }

        public void SetBoolSetting(string key, bool value)
        {
            _values[key] = value.ToString();
        }

        public T GetEnumSetting<T>(string key, T defaultValue) where T : struct, IConvertible
        {
            var stringValue = this.GetStringSetting(key, defaultValue.ToString());
            if (stringValue == null)
            {
                return defaultValue;
            }
            try
            {
                return EnumHelper.Parse<T>(stringValue);
            }
            catch (EnumCastException err)
            {
                throw new FormatException(err.Message, err);
            }
        }

        public void SetEnumSetting<T>(string key, T value) where T : struct, IConvertible
        {
            this.SetStringSetting(key, value.ToString());
        }

        public bool AnyActiveSettings()
        {
            throw new NotSupportedException();
        }

        public void InactivateActiveSettings()
        {
            throw new NotSupportedException();
        }
    }
}
