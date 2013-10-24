﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IDeployStateManager
	{
		DeployState GetDeployState(string deployStateId);
		DeployState CreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId);
		DeployState GetOrCreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId);

		DeployStateMessage AddDeploymentMessage(string deployStateId, string message);

		void MarkDeploymentInProcess(string deployStateId);
		void MarkDeploymentSuccess(string deployStateId);
		void MarkDeploymentFailed(string deployStateId, Exception err);

		void MarkBatchDeploymentSuccess(string deployBatchRequestId);
		void MarkBatchDeploymentFailed(string deployBatchRequestId, Exception err);
		void MarkBatchDeploymentCancelled(string deployBatchRequestId, string cancelMessage);

	}
}
