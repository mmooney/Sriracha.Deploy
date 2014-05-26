using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.ConnectionSettings;
using Raven.Client;
using Sriracha.Deploy.Data;
using MMDB.Shared;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenConnectionSettingRepository : IConnectionSettingRepository
	{
        private readonly IDocumentSession _documentSession;

        private class RavenConnectionSetting<T> where T : ConnectionSettingBase, new()
        {
            public string Id { get; set; }
            public T Value { get; set; }
        }

        public RavenConnectionSettingRepository(IDocumentSession documentSession)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
        }

        private string FormatId<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("userName");
            }
            return typeof(T).Name + "_" + key.Replace('\\', '_');
        }

        public T Load<T>(string key) where T : ConnectionSettingBase, new()
		{
            return _documentSession.LoadEnsureNoCache<RavenConnectionSetting<T>>(FormatId<T>(key)).Value;
		}

        public T Save<T>(T value) where T : ConnectionSettingBase, new()
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var dbItem = _documentSession.Load<RavenConnectionSetting<T>>(FormatId<T>(value.Key));
            if(dbItem == null)
            {
                dbItem = new RavenConnectionSetting<T>
                {
                    Id = FormatId<T>(value.Key),
                    Value = value
                };
                return _documentSession.StoreSaveEvict(dbItem).Value;
            }
            else 
            {
                dbItem.Value = value;
                return _documentSession.SaveEvict(dbItem).Value;
            }
        }
    }
}
