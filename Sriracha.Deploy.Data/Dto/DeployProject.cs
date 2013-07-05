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
		public List<DeployProjectBranch> BranchList { get; set; }
		public List<DeployComponent> ComponentList { get; set; }
		public List<DeployEnvironment> EnvironmentList { get; set; }

		public DeployProject()
		{
			this.BranchList = new List<DeployProjectBranch>();
			this.ComponentList = new List<DeployComponent>();
			this.EnvironmentList = new List<DeployEnvironment>();
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
	}
}
