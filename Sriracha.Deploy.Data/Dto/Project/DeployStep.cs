﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Dto.Project
{
	public enum EnumDeployStepParentType
	{
		Unknown = 0,
		Component = 1, 
		Configuration = 2
	}

	public class DeployStep
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ParentId { get; set; }
		public EnumDeployStepParentType ParentType { get; set; }
		public string StepName { get; set; }
		public string TaskTypeName { get; set; }
		public string TaskOptionsJson { get; set; }
		public string SharedDeploymentStepId { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
        public int OrderNumber { get; set; }
	}
}
