using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Sriracha.Deploy.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.Routes.MapHttpRoute(
				name: "components", 
				routeTemplate: "api/project/{projectId}/component/{componentId}", 
				defaults: new { controller = "Component", componentId = RouteParameter.Optional });

			config.Routes.MapHttpRoute(
				name: "deploymentSteps",
				routeTemplate: "api/project/{projectId}/component/{componentId}/step/{deploymentStepId}",
				defaults: new { controller = "DeploymentStep", deploymentStepId=RouteParameter.Optional });
		}
	}
}
