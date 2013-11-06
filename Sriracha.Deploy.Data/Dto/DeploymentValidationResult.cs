using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeploymentValidationResult
	{
		public class DeploymentValidationResultItem
		{
			public DeployStep DeploymentStep { get; set; }
			public TaskDefinitionValidationResult TaskValidationResult { get; set; }
		}
		public List<DeploymentValidationResultItem> ResultList { get; set; }

		public DeploymentValidationResult()
		{
			this.ResultList = new List<DeploymentValidationResultItem>();
		}

		public DeploymentValidationResultItem AddResult(DeployStep deploymentStep, TaskDefinitionValidationResult validationItem)
		{
			var resultItem = new DeploymentValidationResultItem
			{
				DeploymentStep = deploymentStep,
				TaskValidationResult = validationItem
			};
			this.ResultList.Add(resultItem);
			return resultItem;
		}
	}
}
