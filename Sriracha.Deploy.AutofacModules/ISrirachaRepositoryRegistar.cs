using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.AutofacModules
{
    public interface ISrirachaRepositoryRegistar
    {
        void RegisterRepositories(Autofac.ContainerBuilder builder);
    }
}
