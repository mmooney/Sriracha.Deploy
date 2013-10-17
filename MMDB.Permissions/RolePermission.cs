using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.Permissions
{
	public class RolePermission
	{
		public string Id { get; set; }
		public string PermissionName { get; set; }
		public string RoleId { get; set; }
		public List<PermissionDataAssignment> DataAssignmentList { get; set; }
		public EnumPermissionAccess Access { get; set; }

		public RolePermission()
		{
			this.DataAssignmentList = new List<PermissionDataAssignment>();
		}

		public string GetAssignmentValue(string dataPropertyName)
		{
			if (dataPropertyName == null)
			{
				throw new ArgumentNullException("dataPropertyName is missing");
			}
			var returnValue = this.TryGetAssignmentValue(dataPropertyName);
			if (string.IsNullOrEmpty(returnValue))
			{
				throw new RecordNotFoundException(typeof(PermissionRole), "DataPropertyName", dataPropertyName);
			}
			return returnValue;
		}

		private string TryGetAssignmentValue(string dataObjectName)
		{
			var item = this.DataAssignmentList.FirstOrDefault(i => i.DataPropertyName == dataObjectName);
			if (item == null)
			{
				return null;
			}
			else
			{
				return item.DataPropertyValue;
			}
		}
	}
}
