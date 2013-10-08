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

		public TaskDefinitionValidationResult ValidateTaskDefinition(IDeployTaskDefinition taskDefinition, DeployEnvironmentConfiguration environmentComponent)
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
				result.MachineResultList.Add(machine.Id, machineResultList);
			}
			return result;
		}


		public DeploymentValidationResult ValidateDeployment(DeployProject project, DeployComponent component, DeployEnvironment environment)
		{
			var returnValue = new DeploymentValidationResult();
			DeployEnvironmentConfiguration environmentConfiguration;
			List<DeployStep> deploymentStepList;
			if(component.UseConfigurationGroup)
			{
				environmentConfiguration = environment.TryGetConfigurationItem(component.ConfigurationId);
				var configuration = project.GetConfiguration(component.ConfigurationId);
				deploymentStepList = configuration.DeploymentStepList;;
			}
			else
			{
				environmentConfiguration = environment.TryGetComponentItem(component.Id);
				deploymentStepList = component.DeploymentStepList;
			}
			if(environmentConfiguration != null)
			{
				foreach(var deploymentStep in deploymentStepList)
				{
					var taskDefinition = _taskFactory.CreateTaskDefinition(deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
					var validationItem = this.ValidateTaskDefinition(taskDefinition, environmentConfiguration);
					returnValue.AddResult(deploymentStep, validationItem);
				}
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
			if (!valueDictionary.ContainsKey(parameter.FieldName) || string.IsNullOrEmpty(valueDictionary[parameter.FieldName]))
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


		public ComponentConfigurationDefinition GetComponentConfigurationDefinition(List<DeployStep> deploymentStepList)
		{
			var returnValue = new ComponentConfigurationDefinition();
			if(deploymentStepList != null)
			{
				foreach (var deploymentStep in deploymentStepList)
				{
					var taskDefinition = _taskFactory.CreateTaskDefinition(deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
				
					var environmentList = taskDefinition.GetEnvironmentTaskParameterList();
					returnValue.EnvironmentTaskParameterList.AddRange(environmentList);

					var machineList = taskDefinition.GetMachineTaskParameterList();
					returnValue.MachineTaskParameterList.AddRange(machineList);
				}
			}
			return returnValue;
		}

		public TaskDefinitionValidationResult ValidateMachineTaskDefinition(IDeployTaskDefinition taskDefinition, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine)
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

			var machineResultList = new List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem>();
			foreach (var p in machineParameters)
			{
				var item = this.GetValidationResultItem(p, machine.ConfigurationValueList);
				machineResultList.Add(item);
			}
			result.MachineResultList.Add(machine.Id, machineResultList);

			return result;
		}


		public void ValidateStatusTransition(EnumDeployStatus oldStatus, EnumDeployStatus newStatus)
		{
			//OK
		}
	}
}
