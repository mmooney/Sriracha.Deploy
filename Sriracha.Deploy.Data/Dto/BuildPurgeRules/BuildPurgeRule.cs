using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Dto.BuildPurgeRules
{
	public class BuildPurgeRule 
	{
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public int? BuildRetentionMinutes { get; set; }
        public List<string> EnvironmentIdList { get; set; }
		public List<string> EnvironmentNameList { get; set; }
		public List<string> MachineIdList { get; set; }
		public List<string> MachineNameList { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime UpdatedDateTimeUtc { get; set; }
        public string UpdatedByUserName { get; set; }
		
		public BuildPurgeRule()
		{
			this.EnvironmentIdList = new List<string>();
			this.EnvironmentNameList = new List<string>();
			this.MachineIdList = new List<string>();
			this.MachineNameList = new List<string>();
		}

		public bool MatchesRule(DeployBuild build, IDIFactory diFactory)
		{
			var deployStateRepository = diFactory.CreateInjectedObject<IDeployStateRepository>();
			var projectRepository = diFactory.CreateInjectedObject<IProjectRepository>();
			if (this.EnvironmentIdList != null)
			{
				foreach(var environmentId in this.EnvironmentIdList)
				{
					var list = deployStateRepository.FindDeployStateListForEnvironment(build.Id, environmentId);
					if (list != null && list.Count > 0)
					{
						return true;
					}
				}
			}
			var project = projectRepository.GetProject(build.ProjectId);
			if (this.EnvironmentNameList != null)
			{
				foreach(var environmentName in this.EnvironmentNameList)
				{
					var environmentList = project.EnvironmentList.Where(i=>i.EnvironmentName == environmentName);
					if(environmentList != null)
					{
						foreach(var environment in environmentList)
						{
							var list = deployStateRepository.FindDeployStateListForEnvironment(build.Id, environment.Id);
							if(list != null && list.Count > 0)
							{
								return true;
							}
						}
					}
				}
			}
			if(this.MachineIdList != null)
			{
				foreach(var machineId in this.MachineIdList)
				{
					var machine = project.GetMachine(machineId);
					var list = deployStateRepository.FindDeployStateListForMachine(build.Id, machine.EnvironmentId, machine.Id);
					if(list != null)
					{
						foreach(var item in list)
						{
							if(item.MachineList.Any(i=>i.Id == machine.Id))
							{
								return true;
							}
						}
					}
				}
			}
			if(this.MachineNameList != null)
			{
				foreach(var machineName in this.MachineNameList)
				{
					var machineList = project.GetMachineListForName(machineName);
					if(machineList != null)
					{
						foreach(var machine in machineList)
						{
							var list = deployStateRepository.FindDeployStateListForMachine(build.Id, machine.EnvironmentId, machine.Id);
							if(list != null)
							{
								foreach(var item in list)
								{
									if(item.MachineList.Any(i=>i.Id == machine.Id))
									{
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public string DisplayValue
		{
			get 
			{ 
				var sb = new StringBuilder();
				if(this.EnvironmentIdList != null && this.EnvironmentIdList.Count > 0)
				{
					if(sb.Length > 0)
					{
						sb.Append(",");
					}
					sb.Append(JoinValues("Environment Ids", this.EnvironmentIdList));
				}
				if(this.EnvironmentNameList != null && this.EnvironmentNameList.Count > 0)
				{
					if(sb.Length > 0)
					{
						sb.Append(",");
					}
					sb.Append(JoinValues("Environment Names", this.EnvironmentNameList));
				}
				if(this.MachineIdList != null && this.MachineIdList.Count > 0)
				{
					if(sb.Length > 0)
					{
						sb.Append(",");
					}
					sb.Append(JoinValues("Machine Ids", this.MachineIdList));
				}
				if(this.MachineNameList != null && this.MachineNameList.Count > 0)
				{
					if(sb.Length > 0)
					{
						sb.Append(",");
					}
					sb.Append(JoinValues("Machine Names", this.MachineNameList));
				}
				return sb.ToString();
			}
		}

		private string JoinValues(string description, List<string> valueList)
		{
			return description + ":[" + string.Join(",",valueList) + "]";
		}

    }
}
