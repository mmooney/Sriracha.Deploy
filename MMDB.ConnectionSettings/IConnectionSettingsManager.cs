using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.ConnectionSettings
{
	public interface IConnectionSettingsManager
	{
		T Load<T>(EnumSettingSource source, string key) where T : ConnectionSettingBase, new();
	}
}
