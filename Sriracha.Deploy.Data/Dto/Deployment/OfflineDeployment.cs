using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
	public enum EnumOfflineDeploymentStatus
	{
		Unknown,
		CreateRequested,
		CreateInProcess,
		ReadyForDownload
	}

	public class OfflineDeployment
	{
		public string Id { get; set; }
		public string DeployBatchRequestId { get; set; }
		public EnumOfflineDeploymentStatus Status { get; set; }

		public string CreatedByUserName { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
	}
}
