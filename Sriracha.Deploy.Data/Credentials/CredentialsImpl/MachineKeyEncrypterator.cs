using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials.CredentialsImpl
{
	public class MachineKeyEncrypterator : IEncrypterator
	{
		private const string tempKey = "8cac897b-1774-4c67-91e4-5189e840c4e17329db8a-b5fd-4f74-b568-dd4eabef35ba";

		public string Encrypt(string salt, string decryptedPassword)
		{
			var saltyBytes = Encoding.UTF8.GetBytes(salt);
			return Encryption.AESGCM.SimpleEncryptWithPassword(decryptedPassword, tempKey, saltyBytes);
		}

		public string Decrypt(string salt, string encryptedPassword)
		{
			var saltyBytes = Encoding.UTF8.GetBytes(salt);
			return Encryption.AESGCM.SimpleDecryptWithPassword(encryptedPassword, tempKey, saltyBytes.Length);
		}
	}
}
