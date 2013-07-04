using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IDeployRequestRepository
	{
		Dto.DeployRequest GetRequest(string deployRequestID);
	}
}
