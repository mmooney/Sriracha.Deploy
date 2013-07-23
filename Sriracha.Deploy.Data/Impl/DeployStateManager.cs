using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
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


		public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
		{
			return _deployRepository.AddDeploymentMessage(deployStateId, message);
		}


		public void MarkDeploymentSuccess(string deployStateId)
		{
			_deployRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Success);
		}

		public void MarkDeploymentFailed(string deployStateId, Exception err)
		{
			_deployRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Error, err);
		}
	}
}
