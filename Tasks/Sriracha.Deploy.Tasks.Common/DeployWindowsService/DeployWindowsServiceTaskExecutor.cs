using dropkick.Configuration;
using dropkick.Configuration.Dsl;
using dropkick.Engine;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Tasks.Common.DeployWindowsService.DropkickImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.DeployWindowsService
{
    public class DeployWindowsServiceTaskExecutor : BaseDeployTaskExecutor<DeployWindowsServiceTaskDefinition, DeployWindowsServiceTaskOptions>
    {
        static DeployWindowsServiceTaskExecutor()
        {
            AutoMapper.Mapper.CreateMap<DeployWindowsServiceTaskOptions, ServiceDeploymentSettings>();
        }

        public DeployWindowsServiceTaskExecutor(IParameterEvaluator parameterEvaluator, IDeploymentValidator validator) : base(parameterEvaluator, validator)
        {

        }

        protected override DeployTaskExecutionResult InternalExecute(TaskExecutionContext<DeployWindowsServiceTaskDefinition, DeployWindowsServiceTaskOptions> context)
        {
            context.Info("Starting DeployWindowsServiceTask");

            var deployment = new ServiceDeployment();
            Type settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];

            var settings = AutoMapper.Mapper.Map(context.FormattedOptions, new ServiceDeploymentSettings());

            var newArgs = new DeploymentArguments
            {
                ServerMappings = new RoleToServerMap(),
                Command = DeploymentCommands.Execute
            };
            newArgs.ServerMappings.AddMap("Host", context.FormattedOptions.TargetMachineName);

            //settings.Environment = newArgs.Environment;
            deployment.Initialize(settings);

            DeploymentPlanDispatcher.KickItOutThereAlready(deployment, newArgs);

            context.Info("Done DeployWindowsServiceTask");

            return context.BuildResult();
        }
    }
}
