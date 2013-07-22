using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data
{
	public interface IDeployRunner
	{
		void Deploy(string deployStateId, string environmentId, string buildId, RuntimeSystemSettings systemSettings);
	}
}
