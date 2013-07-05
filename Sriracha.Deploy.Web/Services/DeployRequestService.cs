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

		public DeployRequestService()
		{

		}

		public object Get(DeployRequest request)
		{
			if (request == null ||
				(string.IsNullOrEmpty(request.Id) && string.IsNullOrEmpty(request.ProjectId)))
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(request.Id))
			{
				return _deployRequestManager.InitializeDeployRequest(request.BuildId, request.EnvironmentId);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}