using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IDeployRequestManager
	{
		DeployRequestTemplate InitializeDeployRequest(string p1, string p2);
	}
}
