using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class DeployRequestService : Service
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public DeployRequestService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(DeployRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException();
			}
			//if(!string.IsNullOrEmpty(request.Id))
			//{
			//	throw new NotImplementedException();
			//}
			else 
			{
				if(string.IsNullOrWhiteSpace(request.BuildId) || string.IsNullOrWhiteSpace(request.EnvironmentId))
				{
					throw new ArgumentNullException();
				}
				return _deployRequestManager.InitializeDeployRequest(request.BuildId, request.EnvironmentId);
			}
		}
		public object Put(DeployRequest request)
		{
			return this.PutPost(request);
		}

		public object Post(DeployRequest request)
		{
			return this.PutPost(request);
		}

		private DeployRequest PutPost(DeployRequest request)
		{
			var state = _deployRequestManager.SubmitDeployRequest(request.ProjectId, request.BuildId, request.EnvironmentId, request.MachineIdList);
			request.DeployStateId = state.Id;
			return request;
		}
	}
}