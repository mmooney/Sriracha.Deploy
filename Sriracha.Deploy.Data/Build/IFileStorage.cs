using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Build
{
	public interface IFileStorage
	{
		string StoreFile(byte[] fileData);
		void UpdateFile(string fileStorageId, byte[] fileData);
		byte[] GetFile(string fileStorageId);
        Stream GetFileStream(string fileStorageId);
		void DeleteFile(string fileStorageId);
    }
}
