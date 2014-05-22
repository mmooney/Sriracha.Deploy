using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerSystemSettingsRepository : ISystemSettingsRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;

        public SqlServerSystemSettingsRepository(ISqlConnectionInfo sqlConnectionInfo)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
        }

        [PetaPoco.TableName("SystemSettings")]
        private class SqlSystemSettings
        {
            public string ID { get; set; }
            public bool ActiveIndicator { get; set; }
            public string SettingsJson { get; set; }

            public Dictionary<string,string> GetSettingDictionary()
            {
                if(string.IsNullOrEmpty(this.SettingsJson))
                {
                    return new Dictionary<string,string>();
                }
                else 
                {
                    return JsonConvert.DeserializeObject<Dictionary<string,string>>(this.SettingsJson);
                }
            }
        }

        private SqlSystemSettings EnsureActiveSettings()
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlSystemSettings>("WHERE ActiveIndicator=1");
                if(item == null)
                {
                    item = new SqlSystemSettings
                    {
                        ID = Guid.NewGuid().ToString(),
                        ActiveIndicator = true,
                        SettingsJson = new Dictionary<string,string>().ToJson()
                    };
                    db.Insert("SystemSettings", "ID", false, item);
                }
                return item;
            }
        }

        public string GetStringSetting(string key, string defaultValue)
        {
            var data = EnsureActiveSettings();
            var settings = data.GetSettingDictionary();
            string returnValue;
            if(!settings.TryGetValue(key, out returnValue))
            {
                return defaultValue;
            }
            return returnValue;
        }


        public string SetStringSetting(string key, string value)
        {
            var data = EnsureActiveSettings();
            var settings = data.GetSettingDictionary();
            settings[key] = value;
            data.SettingsJson = settings.ToJson();
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("SystemSettings", "ID", data, data.ID);
            }
            return value;
        }

        public int GetIntSetting(string key, int defaultValue)
        {
            var stringValue = this.GetStringSetting(key, defaultValue.ToString());
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
            var stringValue = this.GetStringSetting(key, defaultValue.ToString());
            bool returnValue;
            if (stringValue == null)
            {
                return defaultValue;
            }
            if (!bool.TryParse(stringValue, out returnValue))
            {
                throw new FormatException(string.Format("Unable to parse {0} value \"{1}\" into an integer", key, stringValue));
            }
            return returnValue;
        }

        public void SetBoolSetting(string key, bool value)
        {
            this.SetStringSetting(key, value.ToString());
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
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM SystemSettings WHERE ActiveIndicator=1");
                return (count != 0);
            }
        }

        public void InactivateActiveSettings()
        {
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("UPDATE SystemSettings SET ActiveIndicator=0");
            }
        }
    }
}
