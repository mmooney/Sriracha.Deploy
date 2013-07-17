using Ninject;
using Ninject.Modules;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.RavenDB;

namespace Sriracha.Deploy.NinjectModules
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
			Bind<IDocumentSession>().ToMethod(context => context.Kernel.Get<IDocumentStore>().OpenSession()).InTransientScope();

			Bind<IBuildRepository>().To<RavenBuildRepository>();
			Bind<IProjectRepository>().To<RavenProjectRepository>();
			Bind<IFileRepository>().To<RavenFileRepository>();
			//Bind<IFileStorage>().To<RavenFileStorage_Collection>();
			Bind<IFileStorage>().To<RavenFileStorage_Attachment>();
			Bind<IRavenAttachmentManager>().To<RavenAttachmentManager>();
			Bind<IDeployHistoryRepository>().To<RavenDeployHistoryRepository>();
		}
	}
}
