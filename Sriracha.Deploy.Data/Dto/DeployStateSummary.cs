using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployStateSummary
	{
		public string Id { get; set; }
		public string DeployBatchRequestItemId { get; set; }
		public string ProjectId { get; set; }
		public DeployBuild Build { get; set; }
		public DeployProjectBranch Branch { get; set; }
		public DeployEnvironment Environment { get; set; }
		public DeployComponent Component { get; set; }
		public List<DeployMachine> MachineList { get; set; }

		public EnumDeployStatus Status { get; set; }
		public string StatusDisplayValue
		{
			get
			{
				return EnumHelper.GetDisplayValue(this.Status);
			}
		}

		public DateTime SubmittedDateTimeUtc { get; set; }
		public DateTime DeploymentStartedDateTimeUtc { get; set; }
		public DateTime DeploymentCompleteDateTimeUtc { get; set; }
		public string ErrorDetails { get; set; }
	}
}
