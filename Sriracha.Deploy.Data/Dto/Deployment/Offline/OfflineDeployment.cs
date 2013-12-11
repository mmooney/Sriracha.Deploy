using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Offline
{
    public enum EnumOfflineDeploymentStatus
    {
        [EnumDisplayValue("Unknown")]
        Unknown,
        [EnumDisplayValue("Create Requested")]
        CreateRequested,
        [EnumDisplayValue("Create In Process")]
        CreateInProcess,
        [EnumDisplayValue("Ready For Download")]
        ReadyForDownload,
        [EnumDisplayValue("Create Failed")]
        CreateFailed
    }

	public class OfflineDeployment
	{
		public string Id { get; set; }
		public string DeployBatchRequestId { get; set; }
		public EnumOfflineDeploymentStatus Status { get; set; }
        public string StatusDisplayValue { get { return EnumHelper.GetDisplayValue(this.Status); } }

        public string FileId { get; set; }
        public string CreateErrorDetails { get; set; }

		public string CreatedByUserName { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }

        public string ResultFileId { get; set; }
    }
}
