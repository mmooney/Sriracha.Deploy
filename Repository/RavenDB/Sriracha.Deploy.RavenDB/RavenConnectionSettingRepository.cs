using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.ConnectionSettings;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenConnectionSettingRepository : IConnectionSettingRepository
	{
		public T Load<T>(string key) where T : ConnectionSettingBase, new()
		{
			throw new NotImplementedException();
		}
	}
}
