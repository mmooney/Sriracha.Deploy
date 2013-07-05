using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public enum EnumRuntimeValidationStatus
	{
		Success,
		Incomplete
	}

	public class TaskDefinitionValidationResult
	{
		public class TaskDefinitionValidationResultItem
		{
			public string FieldName { get; set; }
			public string FieldValue { get; set; }
			public bool Present { get; set; }
			public bool Sensitive { get; set; }
		}

		public List<TaskDefinitionValidationResultItem> EnvironmentResultList { get; set; }
		public Dictionary<string, List<TaskDefinitionValidationResultItem>> MachineResultList { get; set; }

		public TaskDefinitionValidationResult()
		{
			this.EnvironmentResultList = new List<TaskDefinitionValidationResultItem>();
			this.MachineResultList = new Dictionary<string,List<TaskDefinitionValidationResultItem>>();
		}

		public EnumRuntimeValidationStatus Status
		{
			get
			{
				if(this.EnvironmentResultList.Any(i=>!i.Present) || this.MachineResultList.Values.Any(i=>i.Any(j=>!j.Present)))
				{
					return EnumRuntimeValidationStatus.Incomplete;
				}
				else 
				{
					return EnumRuntimeValidationStatus.Success;
				}
			}
		}
	}
}
