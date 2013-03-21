using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data
{
	public class BuildManager
	{
		private IFileRepository FileRepository { get; set; }
		private IBuildRepository BuildRepository { get; set; }

		public BuildManager(IBuildRepository buildRepository, IFileRepository fileRepository)
		{
			this.FileRepository = fileRepository;
			this.BuildRepository = buildRepository;
		}

		public DeployBuild SubmitBuild(string projectId, string branchId, string fileName, byte[] fileData, Version version)
		{
			var file = this.FileRepository.StoreFile(fileName, fileData);
			return this.BuildRepository.StoreBuild(projectId, branchId, file.Id, version);
		}
	}
}
