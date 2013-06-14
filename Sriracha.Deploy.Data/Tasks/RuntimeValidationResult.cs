using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
	public enum EnumRuntimeValidationStatus
	{
		Success,
		Incomplete
	}

	public class RuntimeValidationResult
	{

		public class RuntimeValidationResultItem
		{
			public string FieldName { get; set; }
			public string FieldValue { get; set; }
			public bool Present { get; set; }
			public bool Sensitive { get; set; }
		}

		public List<RuntimeValidationResultItem> EnvironmentResultList { get; set; }
		public Dictionary<string, List<RuntimeValidationResultItem>> MachineResultList { get; set; }

		public RuntimeValidationResult()
		{
			this.EnvironmentResultList = new List<RuntimeValidationResultItem>();
			this.MachineResultList = new Dictionary<string,List<RuntimeValidationResultItem>>();
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
