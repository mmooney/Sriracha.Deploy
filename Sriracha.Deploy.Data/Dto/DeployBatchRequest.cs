using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployBatchRequest
	{
		public string Id { get; set; }
		public DateTime SubmittedDateTimeUtc { get; set; }
		public string SubmittedByUserName { get; set; }
		public List<DeployBatchRequestItem> ItemList { get; set; }
		public EnumDeployStatus Status { get; set; }
		public string StatusDisplayValue { get {  return EnumHelper.GetDisplayValue(this.Status); } }
		public DateTime? StartedDateTimeUtc { get; set; }
		public DateTime? CompleteDateTimeUtc { get; set; }
		public string ErrorDetails { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }

		public List<string> MessageList { get; set; }

		public DeployBatchRequest()
		{
			this.MessageList = new List<string>();
		}

		public string Label { get; set; }
	}
}
