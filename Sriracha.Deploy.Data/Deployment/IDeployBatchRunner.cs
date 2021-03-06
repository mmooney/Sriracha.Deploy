﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IDeployBatchRunner
	{
		void TryRunNextDeployment();
		void ForceRunDeployment(string deployBatchRequestId);
	}
}
