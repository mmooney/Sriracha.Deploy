using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeploymentValidator : IDeploymentValidator
	{
		private readonly IDeployTaskFactory _taskFactory;

		public DeploymentValidator(IDeployTaskFactory taskFactory)
		{
			_taskFactory = taskFactory;
		}

		public TaskDefinitionValidationResult ValidateTaskDefinition(IDeployTaskDefinition taskDefinition, DeployEnvironmentComponent environmentComponent)
		{
			var result = new TaskDefinitionValidationResult();
			//Verify Static Values
			var environmentParmeters = taskDefinition.GetEnvironmentTaskParameterList();
			foreach (var p in environmentParmeters)
			{
				var item = this.GetValidationResultItem(p, environmentComponent.ConfigurationValueList);
				result.EnvironmentResultList.Add(item);
			}
			var machineParameters = taskDefinition.GetMachineTaskParameterList();
			foreach (var machine in environmentComponent.MachineList)
			{
				var machineResultList = new List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem>();
				foreach (var p in machineParameters)
				{
					var item = this.GetValidationResultItem(p, machine.ConfigurationValueList);
					machineResultList.Add(item);
				}
				result.MachineResultList.Add(machine.MachineName, machineResultList);
			}
			return result;
		}


		public DeploymentValidationResult ValidateDeployment(DeployComponent component, DeployEnvironment environment)
		{
			var returnValue = new DeploymentValidationResult();
			var environmentComponent = environment.GetEnvironmentComponent(component.Id);
			foreach(var deploymentStep in component.DeploymentStepList)
			{
				var taskDefinition = _taskFactory.CreateTaskDefinition(deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
				var validationItem = this.ValidateTaskDefinition(taskDefinition, environmentComponent);
				returnValue.AddResult(deploymentStep, validationItem);
			}
			return returnValue;
		}

		private TaskDefinitionValidationResult.TaskDefinitionValidationResultItem GetValidationResultItem(TaskParameter parameter, Dictionary<string, string> valueDictionary)
		{
			var item = new TaskDefinitionValidationResult.TaskDefinitionValidationResultItem
			{
				FieldName = parameter.FieldName,
				Sensitive = parameter.Sensitive
			};
			if (!valueDictionary.ContainsKey(parameter.FieldName))
			{
				item.Present = false;
			}
			else
			{
				item.Present = true;
				//if (!item.Sensitive)
				//{
				item.FieldValue = valueDictionary[parameter.FieldName];
				//}
			}
			return item;
		}
	}
}
