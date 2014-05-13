using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public enum EnumCleanupTaskType
	{
		Folder
	}

	public class CleanupTaskData
	{
		public string Id { get; set; }
		
		public EnumCleanupTaskType TaskType { get; set; }
		public string MachineName { get; set; }
		public string FolderPath { get; set; }

		public int AgeMinutes { get; set; }
		public DateTime TargetCleanUpDateTimeUtc { get; set; }

		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }

		public EnumQueueStatus Status { get; set; }

		public DateTime? StartedDateTimeUtc { get; set; }
		public DateTime? CompletedDateTimeUtc { get; set; }

		public string ErrorDetails { get; set; }
	}
}
