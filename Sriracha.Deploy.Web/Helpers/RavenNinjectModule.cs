using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.RavenDB;

namespace Sriracha.Deploy.Web.Helpers
{
	public class RavenDBNinjectModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IDocumentStore>()
			   .ToMethod(context =>
				   {
					   var documentStore = RavenHelper.CreateDocumentStore();
					   return documentStore;
				   })
				.InSingletonScope();
			Bind<IDocumentSession>().ToMethod(context => context.Kernel.Get<IDocumentStore>().OpenSession()).InRequestScope();

			Bind<IBuildRepository>().To<RavenBuildRepository>();
			Bind<IProjectRepository>().To<RavenProjectRepository>();
		}
	}
}