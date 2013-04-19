using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IFileRepository
	{
		IEnumerable<DeployFile> GetFileList();
		DeployFile CreateFile(string fileName, byte[] data);
		DeployFile GetFile(string fileId);
		DeployFile UpdateFile(string fileId, string fileName, byte[] fileData);
		void DeleteFile(string fileId);
	}
}
