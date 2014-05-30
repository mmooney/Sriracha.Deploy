using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dropkick
{
    public interface IDropkickRunner
    {
        DropkickRunnerContext<TaskDefinitionType, TaskOptionsType> Create<TaskDefinitionType, TaskOptionsType>(TaskExecutionContext<TaskDefinitionType, TaskOptionsType> taskExecutionContext) where TaskDefinitionType : IDeployTaskDefinition;
    }
}
