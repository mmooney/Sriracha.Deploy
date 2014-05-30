using Sriracha.Deploy.Data.Credentials;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dropkick.DropkickImpl
{
    public class DropkickRunner : IDropkickRunner
    {
        private readonly IZipper _zipper;
        private readonly ICredentialsManager _credentialsManager;
        private readonly IProcessRunner _processRunner;
        private readonly IImpersonator _impersonator;

        public DropkickRunner(IZipper zipper, IProcessRunner processRunner, ICredentialsManager credentialsManager, IImpersonator impersonator)
        {
            _zipper = DIHelper.VerifyParameter(zipper);
            _processRunner = DIHelper.VerifyParameter(processRunner);
            _credentialsManager = DIHelper.VerifyParameter(credentialsManager);
            _impersonator = DIHelper.VerifyParameter(impersonator);
        }

        public DropkickRunnerContext<TaskDefinitionType, TaskOptionsType> Create<TaskDefinitionType, TaskOptionsType>(TaskExecutionContext<TaskDefinitionType, TaskOptionsType> taskExecutionContext) where TaskDefinitionType : IDeployTaskDefinition
        {
            string dropkickDirectory = taskExecutionContext.SystemSettings.GetLocalMachineComponentDirectory(taskExecutionContext.Machine.MachineName, taskExecutionContext.Component.Id) + "_Dropkick";
            if (!Directory.Exists(dropkickDirectory))
            {
                Directory.CreateDirectory(dropkickDirectory);
            }
            var dropkickZipName = Path.Combine(dropkickDirectory, "dropkick.zip");
            File.WriteAllBytes(dropkickZipName, SrirachaResources.dropkick_zip);
            _zipper.ExtractFile(dropkickZipName, dropkickDirectory);

            return new DropkickRunnerContext<TaskDefinitionType,TaskOptionsType>(_processRunner, _credentialsManager, _impersonator,  taskExecutionContext, dropkickDirectory);
        }
    }
}
