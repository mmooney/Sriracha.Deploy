﻿using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{

    public class RavenSystemSettingsRepository : ISystemSettingsRepository
    {
        private class SystemSettingsData
        {
            public string Id { get; set; }
            public bool Active { get; set; }
            public Dictionary<string, string> SettingsDictionary { get; set; }
            public List<BaseBuildPurgeRetentionRule> BuildPurgeRetentionRuleList { get; set; }

            public SystemSettingsData()
            {
                this.SettingsDictionary = new Dictionary<string,string>();
            }

        }

        private readonly IDocumentSession _documentSession;

        public RavenSystemSettingsRepository(IDocumentSession documentSession)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
        }

        private SystemSettingsData GetActiveSettingsData()
        {
            return _documentSession.QueryNoCacheNotStale<SystemSettingsData>().FirstOrDefault(i => i.Active);
        }

        public string GetStringSetting(string key, string defaultValue)
        {
            if(string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            var settings = GetActiveSettingsData();
            string returnValue;
            if(settings == null || !settings.SettingsDictionary.TryGetValue(key, out returnValue))
            {
                returnValue = defaultValue;
            }
            return returnValue;
        }

        public string SetStringSetting(string key, string value)
        {
            var item = _documentSession.QueryNotStale<SystemSettingsData>().FirstOrDefault(i => i.Active);
            if(item == null)
            {
                item = new SystemSettingsData
                {
                    Id = Guid.NewGuid().ToString(),
                    Active = true
                };
                _documentSession.Store(item);
            }
            item.SettingsDictionary[key] = value;
            _documentSession.SaveChanges();
            return value;

        }

        public int GetIntSetting(string key, int defaultValue)
        {
            var stringValue = this.GetStringSetting(key, defaultValue.ToString());
            int returnValue;
            if(!int.TryParse(stringValue, out returnValue))
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
            catch(EnumCastException err)
            {
                throw new FormatException(err.Message, err);
            }
        }

        public void SetEnumSetting<T>(string key, T value) where T : struct, IConvertible
        {
            this.SetStringSetting(key, value.ToString());
        }

        public List<BaseBuildPurgeRetentionRule> GetBuildPurgeRetentionRuleList(List<BaseBuildPurgeRetentionRule> defaultValue)
        {
            var item = GetActiveSettingsData();
            if(item == null || item.BuildPurgeRetentionRuleList == null || item.BuildPurgeRetentionRuleList.Count == 0)
            {
                return defaultValue;
            }
            else 
            {
                return item.BuildPurgeRetentionRuleList;
            }
        }

        public List<BaseBuildPurgeRetentionRule> SetBuildPurgeRetentionRuleList(List<BaseBuildPurgeRetentionRule> value)
        {
            var item = _documentSession.QueryNotStale<SystemSettingsData>().FirstOrDefault(i => i.Active);
            if (item == null)
            {
                item = new SystemSettingsData
                {
                    Id = Guid.NewGuid().ToString(),
                    Active = true
                };
                _documentSession.Store(item);
            }
            item.BuildPurgeRetentionRuleList = value;
            _documentSession.SaveChanges();
            return value;
        }


        public bool AnyActiveSettings()
        {
            var x = this.GetActiveSettingsData();
            return (x != null);
        }


        public void InactivateActiveSettings()
        {
            var list = _documentSession.QueryNotStale<SystemSettingsData>().Where(i => i.Active);
            foreach(var item in list)
            {
                item.Active = false;
            }
            _documentSession.SaveChanges();
            foreach (var item in list)
            {
                _documentSession.Advanced.Evict(item);
            }
        }
    }
}
