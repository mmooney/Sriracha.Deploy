using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Web.Services
{
	public class DeployStateService : Service
	{
		private	readonly IDeployStateManager _deployStateManager;

		public DeployStateService(IDeployStateManager deployStateManager)
		{
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
		}

		public object Get(DeployState request)
		{
			if(request == null || string.IsNullOrWhiteSpace(request.Id))
			{
				throw new ArgumentNullException();
			}
			return _deployStateManager.GetDeployState(request.Id);
		}
	}
}