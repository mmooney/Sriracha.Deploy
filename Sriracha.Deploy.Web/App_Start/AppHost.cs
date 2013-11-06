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
using Sriracha.Deploy.Web.Services.SystemLog;
using System.Web;
using ServiceStack.ServiceHost;
using Elmah;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Dto.Project;

//[assembly: WebActivator.PreApplicationStartMethod(typeof(Sriracha.Deploy.Web.App_Start.AppHost), "Start")]

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
	public class AppHost : AppHostBase
	{
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("Sriracha REST API", typeof(ProjectService).Assembly) 
			{ 
			}

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
			ServiceStack.Text.JsConfig.DateHandler = ServiceStack.Text.JsonDateHandler.ISO8601; 		

			//Configure User Defined REST Paths
			Routes
				.Add<DeployProject>("/project")
				.Add<DeployProject>("/project/{id*}")
				.Add<DeployStep>("/project/{projectId}/{parentType}/{parentId}/step")
				.Add<DeployStep>("/project/{projectId}/{parentType}/{parentId}/step/{id*}")
				.Add<DeployConfiguration>("/project/{projectId}/configuration")
				.Add<DeployConfiguration>("/project/{projectId}/configuration/{id*}")
				.Add<DeployComponent>("/project/{projectId}/component")
				.Add<DeployComponent>("/project/{projectId}/component/{id*}")
				.Add<ComponentConfigurationDefinition>("/project/{projectId}/{parentType}/{parentId}/configuration")
				.Add<DeployProjectBranch>("/project/{projectId}/branch")
				.Add<DeployProjectBranch>("/project/{projectId}/branch/{id*}")
				.Add<DeployEnvironment>("/project/{projectId}/environment")
				.Add<DeployEnvironment>("/project/{projectId}/environment/{id*}")
				.Add<DeployProjectRole>("/project/{projectId}/role")
				.Add<DeployProjectRole>("/project/{projectId}/role/{id}")

				.Add<DeployFileDto>("/file")
				.Add<DeployFileDto>("/file/{id*}")

				//.Add<DeployBatchRequest>("/deploy/batchRequest")
				//.Add<DeployBatchRequest>("/deploy/batchRequest/{id}")
				.Add<DeployState>("/deploy/state")
				.Add<DeployState>("/deploy/state/{id}")

				.Add<TaskMetadata>("/taskMetadata")
				.Add<SystemLogRequest>("/systemlog");

			//container.Adapter = NinjectWebCommon.CreateServiceStackAdapter();
			
			//Uncomment to change the default ServiceStack configuration
			//SetConfig(new EndpointHostConfig {
			//});

			//Enable Authentication
			//ConfigureAuth(container);

			//Set MVC to use the same Funq IOC as ServiceStack
			//ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
			this.ServiceExceptionHandler = (request, exception) =>
			{
				//pass the exception over to Elmah
				var context = HttpContext.Current;
				Elmah.ErrorLog.GetDefault(context).Log(new Error(exception, context));

				//call default exception handler or prepare your own custom response
				return DtoUtils.HandleException(this, request, exception);
			};
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