using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Project;

namespace Sriracha.Deploy.Web.Services
{
	public class ComponentService : Service
	{
		private readonly IProjectManager _projectManager;

		public ComponentService(IProjectManager projectManager)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

		public object Get(DeployComponent request)
		{
			if(request == null || 
				(string.IsNullOrEmpty(request.Id) && string.IsNullOrEmpty(request.ProjectId)))
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(request.Id))
			{
				return _projectManager.GetComponent(request.Id);
			}
			else 
			{
				return _projectManager.GetComponentList(request.ProjectId);
			}
		}

		public object Post(DeployComponent component)
		{
			if (string.IsNullOrEmpty(component.Id))
			{
				return _projectManager.CreateComponent(component.ProjectId, component.ComponentName, component.UseConfigurationGroup, component.ConfigurationId);
			}
			else
			{
				return _projectManager.UpdateComponent(component.Id, component.ProjectId, component.ComponentName, component.UseConfigurationGroup, component.ConfigurationId);
			}
		}

		public void Delete(DeployComponent component)
		{
			_projectManager.DeleteComponent(component.ProjectId, component.Id);
		}
	}
}