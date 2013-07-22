using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Web.Mvc;
using ServiceStack.Configuration;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.WebHost.Endpoints;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Web.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Sriracha.Deploy.Web.App_Start.AppHost), "Start")]

//IMPORTANT: Add the line below to MvcApplication.RegisterRoutes(RouteCollection) in the Global.asax:
//routes.IgnoreRoute("api/{*pathInfo}"); 

/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace Sriracha.Deploy.Web.App_Start
{
	//A customizeable typed UserSession that can be extended with your own properties
	//To access ServiceStack's Session, Cache, etc from MVC Controllers inherit from ControllerBase<CustomUserSession>
	public class CustomUserSession : AuthUserSession
	{
		public string CustomProperty { get; set; }
	}

	public class AppHost
		: AppHostBase
	{
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("Sriracha REST API", typeof(ProjectService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
			ServiceStack.Text.JsConfig.DateHandler = ServiceStack.Text.JsonDateHandler.ISO8601; 		

			//Configure User Defined REST Paths
			Routes
				.Add<DeployComponentDeploymentStep>("/project/{projectId}/component/{componentId}/step")
				.Add<DeployComponentDeploymentStep>("/project/{projectId}/component/{componentId}/step/{id*}")
				.Add<DeployComponent>("/project/{projectId}/component")
				.Add<DeployComponent>("/project/{projectId}/component/{id*}")
				.Add<DeployProjectBranch>("/project/{projectId}/branch")
				.Add<DeployProjectBranch>("/project/{projectId}/branch/{id*}")
				.Add<DeployEnvironment>("/project/{projectId}/environment")
				.Add<DeployEnvironment>("/project/{projectId}/environment/{id*}")
				.Add<DeployFileDto>("/file")
				.Add<DeployFileDto>("/file/{id*}")
				.Add<DeployBuild>("/build")
				.Add<DeployBuild>("/build/{id*}")
				.Add<DeployHistory>("/deploy/history")
				.Add<DeployHistory>("/deploy/history{id*}")
				.Add<DeployRequest>("/deployrequest")
				.Add<DeployRequest>("/deployrequest/{id}")
				.Add<DeployState>("/deployState")
				.Add<DeployState>("/deployState/{id}")
				.Add<DeployProject>("/project")
				.Add<DeployProject>("/project/{id*}")
				.Add<TaskMetadata>("/taskMetadata");

			container.Adapter = NinjectWebCommon.CreateServiceStackAdapter();
			
			//Uncomment to change the default ServiceStack configuration
			//SetConfig(new EndpointHostConfig {
			//});

			//Enable Authentication
			//ConfigureAuth(container);

			//Set MVC to use the same Funq IOC as ServiceStack
			//ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
		}

		/* Uncomment to enable ServiceStack Authentication and CustomUserSession
		private void ConfigureAuth(Funq.Container container)
		{
			var appSettings = new AppSettings();

			//Default route: /auth/{provider}
			Plugins.Add(new AuthFeature(() => new CustomUserSession(),
				new IAuthProvider[] {
					new CredentialsAuthProvider(appSettings), 
					new FacebookAuthProvider(appSettings), 
					new TwitterAuthProvider(appSettings), 
					new BasicAuthProvider(appSettings), 
				})); 

			//Default route: /register
			Plugins.Add(new RegistrationFeature()); 

			//Requires ConnectionString configured in Web.Config
			var connectionString = ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString;
			container.Register<IDbConnectionFactory>(c =>
				new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider));

			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.CreateMissingTables();
		}
		*/

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}