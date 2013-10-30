using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials.CredentialsImpl
{
	public class AppSettingKeyEncrypterator : IEncrypterator
	{
		private string GetKey()
		{
			return AppSettingsHelper.GetRequiredSetting("EncryptionKey");
		}

		public string Encrypt(string salt, string decryptedPassword)
		{
			string key = GetKey();
			var saltyBytes = Encoding.UTF8.GetBytes(salt);
			return Encryption.AESGCM.SimpleEncryptWithPassword(decryptedPassword, key, saltyBytes);
		}

		public string Decrypt(string salt, string encryptedPassword)
		{
			string key = GetKey();
			var saltyBytes = Encoding.UTF8.GetBytes(salt);
			return Encryption.AESGCM.SimpleDecryptWithPassword(encryptedPassword, key, saltyBytes.Length);
		}
	}
}
