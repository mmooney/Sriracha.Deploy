using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IFileStorage
	{
		string StoreFile(byte[] fileData);
		void UpdateFile(string fileStorageId, byte[] fileData);
		byte[] GetFile(string fileStorageId);
		void DeleteFile(string fileStorageId);
	}
}
