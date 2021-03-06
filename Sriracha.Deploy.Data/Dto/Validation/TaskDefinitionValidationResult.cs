﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Dto.Validation
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
            public bool Optional { get; set; }
        }

		public List<TaskDefinitionValidationResultItem> EnvironmentResultList { get; set; }
		public Dictionary<string, List<TaskDefinitionValidationResultItem>> MachineResultList { get; set; }
		public List<TaskParameter> BuildParameterList { get; set; }
		public List<TaskParameter> DeployParameterList { get; set; }

		public TaskDefinitionValidationResult()
		{
			this.EnvironmentResultList = new List<TaskDefinitionValidationResultItem>();
			this.MachineResultList = new Dictionary<string,List<TaskDefinitionValidationResultItem>>();
			this.BuildParameterList = new List<TaskParameter>();
			this.DeployParameterList = new List<TaskParameter>();
		}

		public EnumRuntimeValidationStatus Status
		{
			get
			{
				if(this.EnvironmentResultList.Any(i=>!i.Present && !i.Optional) || this.MachineResultList.Values.Any(i=>i.Any(j=>!j.Present && !j.Optional)))
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
