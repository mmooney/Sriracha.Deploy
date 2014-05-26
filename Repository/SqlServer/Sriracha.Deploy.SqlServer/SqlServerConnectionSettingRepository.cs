using MMDB.ConnectionSettings;
using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerConnectionSettingRepository : IConnectionSettingRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;

        [PetaPoco.TableName("ConnectionSetting")]
        private class SqlConnectionSetting
        {
            public string Id { get; set; }
            public string SettingKey { get; set; }
            public string ValueJson { get; set; }
        }

        public SqlServerConnectionSettingRepository(ISqlConnectionInfo sqlConnectionInfo)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
        }

        public T Load<T>(string key) where T : ConnectionSettingBase, new()
        {
            if(string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlConnectionSetting>("FROM ConnectionSetting WHERE SettingKey=@0", key);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(T), "Key", key);
                }
                return JsonConvert.DeserializeObject<T>(item.ValueJson);
            }
        }

        public T Save<T>(T value) where T : ConnectionSettingBase, new()
        {
            if(value == null)
            {
                throw new ArgumentNullException("value");
            }
            if(string.IsNullOrEmpty(value.Key))
            {
                throw new ArgumentNullException("value.Key");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlConnectionSetting>("FROM ConnectionSetting WHERE SettingKey=@0", value.Key);
                if (item == null)
                {
                    item = new SqlConnectionSetting
                    {
                        Id = Guid.NewGuid().ToString(),
                        SettingKey = value.Key,
                        ValueJson = value.ToJson()
                    };
                    db.Insert("ConnectionSetting", "ID", false, item);
                }
                else 
                {
                    item.ValueJson = value.ToJson();
                    db.Update("ConnectionSetting", "ID", item, item.Id);
                }
            }
            return this.Load<T>(value.Key);
        }
    }
}
