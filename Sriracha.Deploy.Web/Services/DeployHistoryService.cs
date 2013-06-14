using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class DeployHistoryService : Service
	{
		private readonly IDeployHistoryManager _deployHistoryManager;

		public DeployHistoryService(IDeployHistoryManager deployHistoryManager)
		{
			_deployHistoryManager = DIHelper.VerifyParameter(deployHistoryManager);
		}

		public object Get(DeployHistory request)
		{
			if(request != null && !string.IsNullOrEmpty(request.Id))
			{
				return _deployHistoryManager.GetDeployHistory(request.Id);
			}
			else 
			{
				string buildId = null;
				string environmentId = null;
				if(request != null)
				{
					buildId = request.BuildId;
					environmentId = request.EnvironmentId;
				}
				return _deployHistoryManager.GetDeployHistoryList(request.BuildId, request.EnvironmentId);
			}
		}
	}
}