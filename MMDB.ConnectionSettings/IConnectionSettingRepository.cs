using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.ConnectionSettings
{
	public interface IConnectionSettingRepository
	{
		T Load<T>(string key) where T : ConnectionSettingBase, new();

        T Save<T>(T value) where T : ConnectionSettingBase, new();
    }
}
