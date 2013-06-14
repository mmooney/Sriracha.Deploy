using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IDeployHistoryRepository
	{
		IEnumerable<DeployHistory> GetDeployHistoryList(string buildId, string environmentId);
		DeployHistory GetDeployHistory(string deployHistoryId);
	}
}
