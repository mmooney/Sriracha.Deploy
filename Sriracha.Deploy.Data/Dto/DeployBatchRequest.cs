using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployBatchRequest
	{
		public string Id { get; set; }
		public List<DeployBatchRequestItem> ItemList { get; set; }
	}
}
