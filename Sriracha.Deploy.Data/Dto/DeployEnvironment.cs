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
		public List<DeployEnvironmentComponent> ComponentList { get; set; }

		public DeployEnvironment()
		{
			this.ComponentList = new List<DeployEnvironmentComponent>();
		}

		public DeployEnvironmentComponent GetEnvironmentComponent(string componentId)
		{
			if(this.ComponentList == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentComponent), "ComponentId", componentId);
			}
			var returnValue = this.TryGetEnvironmentComponent(componentId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentComponent), "ComponentId", componentId);
			}
			return returnValue;
		}

		public DeployEnvironmentComponent TryGetEnvironmentComponent(string componentId)
		{
			if(this.ComponentList != null)
			{
				return this.ComponentList.SingleOrDefault(i=>i.ComponentId == componentId);
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
			return machineList;
		}
	}
}
