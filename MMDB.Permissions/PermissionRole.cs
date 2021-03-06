﻿using MMDB.Shared;
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

		public string GetAssignmentValue(string dataPropertyName)
		{
			if(dataPropertyName == null)
			{
				throw new ArgumentNullException("dataObjectName is missing");
			}
			var returnValue = this.TryGetAssignmentValue(dataPropertyName);
			if(string.IsNullOrEmpty(returnValue))
			{
				throw new RecordNotFoundException(typeof(PermissionRole), "DataPropertyName", dataPropertyName);
			}
			return returnValue;
		}

		private string TryGetAssignmentValue(string dataPropertyName)
		{
			var item = this.DataAssignmentList.FirstOrDefault(i=>i.DataPropertyName == dataPropertyName);
			if(item == null)
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
