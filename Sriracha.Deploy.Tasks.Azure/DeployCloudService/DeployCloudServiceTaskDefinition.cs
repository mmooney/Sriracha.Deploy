using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    public class DeployCloudServiceTaskDefinition : BaseDeployTaskDefinition<DeployCloudServiceTaskTaskOptions, DeployCloudServiceTaskExecutor>
    {
        public override IList<TaskParameter> GetStaticTaskParameterList()
        {
            throw new NotImplementedException();
        }

        public override IList<TaskParameter> GetEnvironmentTaskParameterList()
        {
            throw new NotImplementedException();
        }

        public override IList<TaskParameter> GetMachineTaskParameterList()
        {
            throw new NotImplementedException();
        }

        public override IList<TaskParameter> GetBuildTaskParameterList()
        {
            throw new NotImplementedException();
        }

        public override IList<TaskParameter> GetDeployTaskParameterList()
        {
            throw new NotImplementedException();
        }

        public override string TaskDefintionName
        {
            get { throw new NotImplementedException(); }
        }
    }
}
