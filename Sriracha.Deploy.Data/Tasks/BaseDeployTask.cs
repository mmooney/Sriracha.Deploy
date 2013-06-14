using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTask<TaskOptions> : IDeployTask
		where TaskOptions : new()
	{
		public TaskOptions Options { get; set; }
		
		public BaseDeployTask()
		{
			this.Options = new TaskOptions();
		}

		public abstract IList<TaskParameter> GetStaticTaskParameterList();
		public abstract IList<TaskParameter> GetEnvironmentTaskParameterList();
		public abstract IList<TaskParameter> GetMachineTaskParameterList();

		public RuntimeValidationResult ValidateRuntimeValues(DeployEnvironmentComponent environmentComponent)
		{
			var result = new RuntimeValidationResult();
			//Verify Static Values
			var environmentParmeters = this.GetEnvironmentTaskParameterList();
			foreach(var p in environmentParmeters)
			{
				var item = this.GetValidationResultItem(p, environmentComponent.ConfigurationValueList);
				result.EnvironmentResultList.Add(item);
			}
			var machineParameters = this.GetMachineTaskParameterList();
			foreach(var machine in environmentComponent.MachineList)
			{
				var machineResultList = new List<RuntimeValidationResult.RuntimeValidationResultItem>();
				foreach(var p in machineParameters)
				{
					var item = this.GetValidationResultItem(p, machine.ConfigurationValueList);
					machineResultList.Add(item);
				}
				result.MachineResultList.Add(machine.MachineName, machineResultList);
			} 
			return result;
		}

		private RuntimeValidationResult.RuntimeValidationResultItem GetValidationResultItem(TaskParameter parameter, Dictionary<string, string> valueDictionary)
		{
			var item = new RuntimeValidationResult.RuntimeValidationResultItem
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
				if (!item.Sensitive)
				{
					item.FieldValue = valueDictionary[parameter.FieldName];
				}
			}
			return item;
		}
	}
}
