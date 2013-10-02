using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployProject
	{
		public string Id { get; set; }
		public string ProjectName { get; set; }
		public bool UsesSharedComponentConfiguration { get; set; }
		public List<DeployProjectBranch> BranchList { get; set; }
		public List<DeployComponent> ComponentList { get; set; }
		public List<DeployEnvironment> EnvironmentList { get; set; }
		public List<DeployConfiguration> ConfigurationList { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }

		public DeployProject()
		{
			this.BranchList = new List<DeployProjectBranch>();
			this.ComponentList = new List<DeployComponent>();
			this.EnvironmentList = new List<DeployEnvironment>();
			this.ConfigurationList = new List<DeployConfiguration>();
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

		public List<DeployMachine> GetMachineListForName(string machineName)
		{
			List<DeployMachine> machineList = new List<DeployMachine>();
			if (this.EnvironmentList != null)
			{
				foreach(var environment in this.EnvironmentList)
				{
					var tempList = environment.GetMachineListForName(machineName);
					machineList.AddRange(tempList);
				}
			}
			return null;
		}

		private DeployMachine TryGetMachine(string machineId)
		{
			if(this.EnvironmentList != null)
			{
				var environment = this.EnvironmentList.First(i=>i.HasMachine(machineId));
				if(environment != null)
				{
					return environment.GetMachine(machineId);
				}
			}
			return null;
		}

		public DeployEnvironment GetEnvironment(string environmentId)
		{
			var returnValue = this.EnvironmentList.SingleOrDefault(i=>i.Id == environmentId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environmentId);
			}
			return returnValue;
		}

		public DeployComponent GetComponent(string componentId)
		{
			var returnValue = this.ComponentList.SingleOrDefault(i=>i.Id == componentId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
			}
			return returnValue;
		}

		public DeployConfiguration GetConfiguration(string configurationId)
		{
			var returnValue = this.ConfigurationList.SingleOrDefault(i=>i.Id == configurationId);
			if(returnValue != null)
			{
				throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
			}
			return returnValue;
		}

		public DeployProjectBranch GetBranch(string branchId)
		{
			var returnValue = this.BranchList.SingleOrDefault(i=>i.Id == branchId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
			}
			return returnValue;
		}
	}
}
