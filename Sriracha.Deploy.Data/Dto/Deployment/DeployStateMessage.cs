﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
	public class DeployStateMessage
	{
		public string Id { get; set; }
		public string DeployStateId { get; set; }
		public string Message { get; set; }
		public DateTime DateTimeUtc { get; set; }
		public string MessageUserName { get; set; }
	}
}