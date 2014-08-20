using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dropkick;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Tasks.Common.DropkickImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.DeployWebsite
{
    public class DeployWebsiteTaskExecutor : BaseDeployTaskExecutor<DeployWebsiteTaskDefinition, DeployWebsiteTaskOptions>
    {
        private readonly IDropkickRunner _dropkickRunner;

        static DeployWebsiteTaskExecutor()
        {
            AutoMapper.Mapper.CreateMap<DeployWebsiteTaskOptions, CommonDeploymentSettings>();
        }
        public DeployWebsiteTaskExecutor(IParameterEvaluator parameterEvaluator, IDeploymentValidator validator, IDropkickRunner dropkickRunner)
            : base(parameterEvaluator, validator)
        {
            _dropkickRunner = DIHelper.VerifyParameter(dropkickRunner);
        }

        protected override DeployTaskExecutionResult InternalExecute(TaskExecutionContext<DeployWebsiteTaskDefinition, DeployWebsiteTaskOptions> context)
        {
            context.Info("Starting DeployWebsiteTask");

            var deployment = new CommonDeployment();
            Type settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];

            var settings = AutoMapper.Mapper.Map(context.FormattedOptions, new CommonDeploymentSettings());

            using (var dropkickContext = _dropkickRunner.Create(context))
            {
                var serverMap = deployment.GetDefaultServerMap();
                serverMap["Website"] = context.FormattedOptions.TargetMachineName;
                dropkickContext.Run<CommonDeployment>(settings, serverMap, "Website".ListMe());

                context.Info("Done DeployWebsiteTask");

                return context.BuildResult();
            }
        }
    }
}
