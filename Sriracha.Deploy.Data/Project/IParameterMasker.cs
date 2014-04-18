using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Project
{
    public interface IParameterMasker
    {
        DeployProject Mask(DeployProject project);
        IEnumerable<DeployProject> Mask(IEnumerable<DeployProject> projectList);

        DeployEnvironment Mask(DeployProject project, DeployEnvironment environment);
        IEnumerable<DeployEnvironment> Mask(DeployProject project, IEnumerable<DeployEnvironment> environmentList);

        DeployEnvironmentConfiguration Mask(DeployProject project, DeployEnvironmentConfiguration environmentConfiguration);
        IEnumerable<DeployEnvironmentConfiguration> Mask(DeployProject project, IEnumerable<DeployEnvironmentConfiguration> environmentConfigurationList);

        DeployEnvironment Unmask(DeployProject project, DeployEnvironment environment, DeployEnvironment originalEnvironment);
        IEnumerable<DeployEnvironmentConfiguration> Unmask(DeployProject project, IEnumerable<DeployEnvironmentConfiguration> environmentConfigurationList, DeployEnvironment originalEnvironment);
        DeployEnvironmentConfiguration Unmask(DeployProject project, DeployEnvironmentConfiguration environmentConfiguration, DeployEnvironment originalEnvironment);
    }
}
