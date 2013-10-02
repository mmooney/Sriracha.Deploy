using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployEnvironmentConfiguration
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentId { get; set; }
		public string ParentId { get; set; }
		public EnumDeployStepParentType ParentType { get; set; }
		public List<DeployMachine> MachineList { get; set; }
		public Dictionary<string, string> ConfigurationValueList { get; set; }

		public DeployEnvironmentConfiguration()
		{
			this.MachineList = new List<DeployMachine>();
			this.ConfigurationValueList = new Dictionary<string,string>();
		}

		public  DeployMachine GetMachine(string machineId)
		{
			var returnValue = this.MachineList.SingleOrDefault(i=>i.Id == machineId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployMachine), "Id", machineId);
			}
			return returnValue;
		}
	}
}
