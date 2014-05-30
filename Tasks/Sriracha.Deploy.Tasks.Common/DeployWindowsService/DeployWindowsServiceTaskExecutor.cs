﻿using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dropkick;
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
        private readonly IDropkickRunner _dropkickRunner;

        static DeployWindowsServiceTaskExecutor()
        {
            AutoMapper.Mapper.CreateMap<DeployWindowsServiceTaskOptions, ServiceDeploymentSettings>();
        }

        public DeployWindowsServiceTaskExecutor(IParameterEvaluator parameterEvaluator, IDeploymentValidator validator, IDropkickRunner dropkickRunner) : base(parameterEvaluator, validator)
        {
            _dropkickRunner = DIHelper.VerifyParameter(dropkickRunner);
        }

        protected override DeployTaskExecutionResult InternalExecute(TaskExecutionContext<DeployWindowsServiceTaskDefinition, DeployWindowsServiceTaskOptions> context)
        {
            context.Info("Starting DeployWindowsServiceTask");

            var deployment = new ServiceDeployment();
            Type settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];

            var settings = AutoMapper.Mapper.Map(context.FormattedOptions, new ServiceDeploymentSettings());

            using(var dropkickContext = _dropkickRunner.Create(context))
            {
                var serverMap = new Dictionary<string, string>();
                serverMap.Add("Host", context.FormattedOptions.TargetMachineName);
                dropkickContext.Run<ServiceDeployment>(settings, serverMap);

                context.Info("Done DeployWindowsServiceTask");

                return context.BuildResult();
            }
        }
    }
}
