using Autofac;
using Raven.Client;
using Sriracha.Deploy.AutofacModules;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Credentials;

namespace Sriracha.Deploy.RavenDB.DataPatcher
{
    public class Program
    {
        private static IDIFactory _diFactory;
        private static NLog.Logger _logger;

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SrirachaAutofacorator(EnumDIMode.Service));
            var container = builder.Build();
            _logger = container.Resolve<NLog.Logger>();
            _diFactory = container.Resolve<IDIFactory>();

            var session = _diFactory.CreateInjectedObject<IDocumentSession>();

			var x = session.Query<DeployCredentials>().FirstOrDefault(i=>i.UserName == "mmoone00c");
			if(x != null)
			{
				session.Delete(x);
				session.SaveChanges();
			}

            bool done = false;
            int processedRecords = 0;
            while(!done) 
            {  
                RavenQueryStatistics stats;
                var list = session.Query<DeployBuild>()
                    .Statistics(out stats)
                    .OrderBy(i=>i.CreatedDateTimeUtc)
                    .Skip(processedRecords)
                    .ToList();
                foreach(var build in list)
                {
                    string oldVersion = build.Version;
                    build.Version = "test";
                    build.Version = oldVersion;
                    session.SaveEvict(build);
                }
                processedRecords += list.Count;
                if (processedRecords >= stats.TotalResults)
                {
                    done = true;
                }
            }

            done = false;
            while(!done)
            {
                var processedRecordList = new List<string>();
                RavenQueryStatistics stats;
                var list = session.Query<DeployState>()
                    .Statistics(out stats)
                    .OrderBy(i => i.CreatedDateTimeUtc)
                    .Take(int.MaxValue)
                    .Skip(processedRecordList.Count)
                    .ToList();
                foreach (var state in list)
                {
                    if(processedRecordList.Contains(state.Id))
                    {
                        throw new Exception("Duplicatge: " + state.Id);
                    }
                    if(state.Build != null)
                    {
                        string oldVersion = state.Build.Version;
                        state.Build.Version = "test";
                        state.Build.Version = oldVersion;
                        //session.SaveEvict(state);
                    }
                    processedRecordList.Add(state.Id);
                }
                session.SaveChanges();
                if (processedRecordList.Count >= stats.TotalResults)
                {
                    done = true;
                }
            }
        }
    }
}
