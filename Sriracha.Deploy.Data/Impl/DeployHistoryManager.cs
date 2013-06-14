using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployHistoryManager : IDeployHistoryManager
	{
		private readonly IDeployHistoryRepository _deployHistoryRepository;

		public DeployHistoryManager(IDeployHistoryRepository deployHistoryRepository)
		{
			this._deployHistoryRepository = DIHelper.VerifyParameter(deployHistoryRepository);
		}

		public IEnumerable<DeployHistory> GetDeployHistoryList(string buildId, string environmentId)
		{
			return _deployHistoryRepository.GetDeployHistoryList(buildId, environmentId);
		}

		public Dto.DeployHistory GetDeployHistory(string deployHistoryId)
		{
			return _deployHistoryRepository.GetDeployHistory(deployHistoryId);
		}
	}
}
