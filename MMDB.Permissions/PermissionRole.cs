using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public class PermissionRole
	{
		public string Id { get; set; }
		public string RoleName { get; set; }
		public List<PermissionDataAssignment> DataAssignmentList { get; set; }

		public PermissionRole()
		{
			this.DataAssignmentList = new List<PermissionDataAssignment>();
		}

		public string GetAssignmentValue(string dataObjectName)
		{
			if(dataObjectName == null)
			{
				throw new ArgumentNullException("dataObjectName is missing");
			}
			var returnValue = this.TryGetAssignmentValue(dataObjectName);
			if(string.IsNullOrEmpty(returnValue))
			{
				throw new RecordNotFoundException(typeof(PermissionRole), "DataObjectName", dataObjectName);
			}
			return returnValue;
		}

		private string TryGetAssignmentValue(string dataObjectName)
		{
			var item = this.DataAssignmentList.FirstOrDefault(i=>i.DataObjectName == dataObjectName);
			if(item == null)
			{
				return null;
			}
			else 
			{
				return item.DataObjectId;
			}
		}
	}
}
