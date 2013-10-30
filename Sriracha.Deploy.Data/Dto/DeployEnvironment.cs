using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployEnvironment
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentName { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public List<DeployEnvironmentConfiguration> ComponentList { get; set; }
		public List<DeployEnvironmentConfiguration> ConfigurationList { get; set; }
		public string DeploymentCredentialsUserName { get; set; }

		public DeployEnvironment()
		{
			this.ComponentList = new List<DeployEnvironmentConfiguration>();
			this.ConfigurationList = new List<DeployEnvironmentConfiguration>();
		}

		public DeployEnvironmentConfiguration GetComponentItem(string componentId)
		{
			if(this.ComponentList == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentConfiguration), "ComponentId", componentId);
			}
			var returnValue = this.TryGetComponentItem(componentId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentConfiguration), "ComponentId", componentId);
			}
			return returnValue;
		}

		public DeployEnvironmentConfiguration TryGetComponentItem(string componentId)
		{
			if(this.ComponentList != null)
			{
				return this.ComponentList.SingleOrDefault(i=>i.ParentId == componentId);
			}
			else 
			{
				return null;
			}
		}

		public DeployEnvironmentConfiguration GetConfigurationItem(string configurationId)
		{
			var returnValue = this.TryGetConfigurationItem(configurationId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentConfiguration), "Id", configurationId);
			}
			return returnValue;
		}

		public DeployEnvironmentConfiguration TryGetConfigurationItem(string configurationId)
		{
			if(this.ConfigurationList != null)
			{
				return this.ConfigurationList.SingleOrDefault(i=>i.ParentId == configurationId);
			}
			else
			{ 
				return null;
			}
		}

		public bool HasMachine(string machineId)
		{
			return (this.TryGetMachine(machineId) != null);
		}

		public DeployMachine GetMachine(string machineId)
		{
			var returnValue = this.TryGetMachine(machineId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployMachine), "Id", machineId);
			}
			return returnValue;
		}

		private DeployMachine TryGetMachine(string machineId)
		{
			if(this.ComponentList != null)
			{
				var component = this.ComponentList.FirstOrDefault(i=>i.MachineList.Any(j=>j.Id == machineId));
				if(component != null)
				{
					return component.MachineList.First(i=>i.Id == machineId);
				}
				else
				{
					var configuration = this.ConfigurationList.FirstOrDefault(i=>i.MachineList.Any(j=>j.Id == machineId));
					if(configuration != null)
					{
						return configuration.MachineList.First(i=>i.Id == machineId);
					}
				}
			}
			return null;
		}

		public List<DeployMachine> GetMachineListForName(string machineName)
		{
			List<DeployMachine> machineList = new List<DeployMachine>();
			if(this.ComponentList != null)
			{
				foreach(var component in this.ComponentList)
				{
					var tempList = component.MachineList.Where(i=>i.MachineName == machineName);
					machineList.AddRange(tempList);
				}
			}
			if(this.ConfigurationList != null)
			{
				foreach(var configuration in this.ConfigurationList)
				{
					var tempList = configuration.MachineList.Where(i=>i.MachineName == machineName);
					machineList.AddRange(tempList);
				}
			}
			return machineList;
		}
	}
}
