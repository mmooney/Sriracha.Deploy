using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IDeployQueueManager
	{
		PagedSortedList<DeployBatchRequest> GetQueue(ListOptions listOptions);
	}
}
