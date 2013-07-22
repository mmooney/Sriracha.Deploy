using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployStateManager : IDeployStateManager
	{
		private readonly IDeployRepository _deployRepository;

		public DeployStateManager(IDeployRepository deployRepository)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
		}

		public DeployState GetDeployState(string deployStateId)
		{
			return _deployRepository.GetDeployState(deployStateId);
		}

		public DeployState PopNextDeployment()
		{
			return _deployRepository.PopNextDeployment();
		}
	}
}
