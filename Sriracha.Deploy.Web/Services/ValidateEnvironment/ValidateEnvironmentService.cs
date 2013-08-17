using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.Web.Services.ValidateEnvironment
{
	public class ValidateEnvironmentService : Service 
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public ValidateEnvironmentService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(ValidateEnvironmentRequest request)
		{
			if(request == null || string.IsNullOrWhiteSpace(request.BuildId) || string.IsNullOrWhiteSpace(request.EnvironmentId))
			{
				throw new ArgumentNullException();
			}
			var deployRequest = _deployRequestManager.InitializeDeployRequest(request.BuildId, request.EnvironmentId);
			return new ValidateEnvironmentResponse
			{
				Environment = deployRequest.Environment,
				Build = deployRequest.Build,
				Component = deployRequest.Component,
				ValidationResult = deployRequest.ValidationResult
			};
		}
	}
}