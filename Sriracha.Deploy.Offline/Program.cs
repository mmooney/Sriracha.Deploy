using Autofac;
using Sriracha.Deploy.AutofacModules;
using Sriracha.Deploy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sriracha.Deploy.Offline
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SrirachaAutofacorator(EnumDIMode.Offline));
            var container = builder.Build();
            var diFactory = container.Resolve<IDIFactory>();

            //var dataProvider = diFactory.CreateInjectedObject<Sriracha.Deploy.Data.Deployment.Offline.IOfflineDataProvider>();
            //dataProvider.Initialize(new Data.Dto.Deployment.DeployBatchRequest { Id = "test" });
            //var batchRunner = diFactory.CreateInjectedObject<Sriracha.Deploy.Data.Deployment.IDeployBatchRunner>(); 
            //var deployRepository = diFactory.CreateInjectedObject<Sriracha.Deploy.Data.Repository.IDeployRepository>(parameters);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(diFactory));
        }
    }
}
