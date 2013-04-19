using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class BuildService : Service
	{
		private readonly IBuildManager _buildManager;

		public BuildService(IBuildManager buildManager)
		{
			this._buildManager = buildManager;
		}

		public object Get(DeployBuild request)
		{
			return _buildManager.GetBuildList();
		}

	}
}