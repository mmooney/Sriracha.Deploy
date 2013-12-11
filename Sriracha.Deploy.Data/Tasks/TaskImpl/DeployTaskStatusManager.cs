using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Deployment;

namespace Sriracha.Deploy.Data.Tasks.TaskImpl
{
	public class DeployTaskStatusManager : IDeployTaskStatusManager
	{
		private readonly Logger _logger;
		private readonly IDeployStateManager _deployStateManager;
        private readonly IDeployStatusNotifier _deployTaskStatusNotifier;

		private List<string> DebugList { get; set; }
		private List<string> InfoList { get; set; }
		private List<string> ErrorList { get; set; }

		public DeployTaskStatusManager(Logger logger, IDeployStateManager deployStateManager, IDeployStatusNotifier deployTaskStatusNotififer)
		{
			this.InfoList = new List<string>();
			this.DebugList = new List<string>();
			this.ErrorList = new List<string>();

			_logger = DIHelper.VerifyParameter(logger);
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
            _deployTaskStatusNotifier = DIHelper.VerifyParameter(deployTaskStatusNotififer);
		}

		public DeployTaskExecutionResult BuildResult()
		{
			var returnValue = new DeployTaskExecutionResult();
			if(this.ErrorList.Any())
			{
				returnValue.Status = EnumDeployTaskExecutionResultStatus.Error;
			}
			else 
			{
				returnValue.Status = EnumDeployTaskExecutionResultStatus.Success;
			}
			return returnValue;
		}

		private void AddMessageToObject(string deployStateId, string prefix, string message)
		{
			var state = _deployStateManager.AddDeploymentMessage(deployStateId, string.Format("{0}: {1}", prefix, message));
            _deployTaskStatusNotifier.Notify(state);
		}

		public void Debug(string deployStateId, string message)
		{
			this._logger.Debug("Deployment {0}: {1}", deployStateId, message);
			this.DebugList.Add(message);
			this.AddMessageToObject(deployStateId, "DEBUG", message);
		}

		public void Info(string deployStateId, string message)
		{
			this._logger.Info("Deployment {0}: {1}", deployStateId, message);
			this.InfoList.Add(message);
			this.AddMessageToObject(deployStateId, "INFO", message);
		}

		public void Error(string deployStateId, string message)
		{
			this._logger.Error("Deployment {0}: {1}", deployStateId, message);
			this.ErrorList.Add(message);
			this.AddMessageToObject(deployStateId, "ERROR", message);
		}

		public void Error(string deployStateId, Exception err)
		{
			this._logger.ErrorException(string.Format("Deployment {0}: {1}", deployStateId, err.ToString()), err);
			this.ErrorList.Add(err.ToString());
			this.AddMessageToObject(deployStateId, "ERROR", err.ToString());
		}
	}
}
