using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using System.IO;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IFileRepository
	{
		List<DeployFile> GetFileList();
		DeployFile CreateFile(string fileName, byte[] data, FileManifest fileManifest);
		DeployFile GetFile(string fileId);
        DeployFile UpdateFile(string fileId, string fileName, byte[] fileData, FileManifest fileManifest);
		DeployFile DeleteFile(string fileId);

        byte[] GetFileData(string fileId);
        Stream GetFileDataStream(string fileId);
    }
}
