using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
	public class DeployBatchRequest
	{
		public string Id { get; set; }
		public DateTime SubmittedDateTimeUtc { get; set; }
		public string SubmittedByUserName { get; set; }
		public List<DeployBatchRequestItem> ItemList { get; set; }
		public EnumDeployStatus Status { get; set; }
		public DateTime? StartedDateTimeUtc { get; set; }
		public DateTime? CompleteDateTimeUtc { get; set; }
		public string ErrorDetails { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public string LastStatusMessage { get; set; }
		public string DeploymentLabel { get; set; }
		public bool CancelRequested { get; set; }
		public string CancelMessage { get; set; }

		public List<string> MessageList { get; set; }

		public string StatusDisplayValue 
		{ 
			get 
			{ 
				string displayValue = EnumHelper.GetDisplayValue(this.Status); 
				if(this.CancelRequested)
				{
					displayValue += " (Cancel Requested)";
				}
				if(this.ResumeRequested)
				{
					displayValue += " (Resume Requested)";
				}
				return displayValue;
			} 
		}

		public DeployBatchRequest()
		{
			this.MessageList = new List<string>();
		}


		public bool ResumeRequested { get; set; }
		public string ResumeMessage { get; set; }

		public Plan.DeploymentPlan Plan { get; set; }
	}
}
