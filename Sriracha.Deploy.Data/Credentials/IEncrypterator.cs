using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials
{
	public interface IEncrypterator
	{
		string Encrypt(string salt, string decryptedPassword);
		string Decrypt(string salt, string encryptedPassword);
	}
}
