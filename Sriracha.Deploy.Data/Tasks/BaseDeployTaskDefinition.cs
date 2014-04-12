using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTaskDefinition<TaskOptions, TaskExecutor> : IDeployTaskDefinition
		where TaskExecutor : IDeployTaskExecutor  
		where TaskOptions : new()
				
	{
        private readonly IParameterParser _parameterParser;
        
        public TaskOptions Options { get; set; }

		public BaseDeployTaskDefinition(IParameterParser parameterParser)
		{
			this.Options = new TaskOptions();
            _parameterParser = DIHelper.VerifyParameter(parameterParser);
		}

		public Type GetTaskOptionType()
		{
			return typeof(TaskOptions);
		}

		public object DeployTaskOptions
		{
			get
			{
				return this.Options;
			}
			set
			{
				if(value == null)
				{
					this.Options = default(TaskOptions);
				}
				else 
				{
					if(!typeof(TaskOptions).IsInstanceOfType(value))
					{
						throw new Exception(string.Format("DeployTaskOptions must be of type {0}, was {1}", typeof(TaskOptions).FullName, value.GetType().FullName));
					}
					this.Options = (TaskOptions)value;
				}
			}
		}

        public virtual IList<TaskParameter> GetStaticTaskParameterList()
        {
            return new List<TaskParameter>();
        }

        public virtual IList<TaskParameter> GetEnvironmentTaskParameterList()
        {
            return _parameterParser.FindNestedEnvironmentParameters(this.Options);
        }

        public virtual IList<TaskParameter> GetMachineTaskParameterList()
        {
            return _parameterParser.FindNestedMachineParameters(this.Options);
        }

        public virtual IList<TaskParameter> GetBuildTaskParameterList()
        {
            return _parameterParser.FindNestedBuildParameters(this.Options);
        }

        public virtual IList<TaskParameter> GetDeployTaskParameterList()
        {
            return _parameterParser.FindNestedDeployParameters(this.Options);
        }
        
        public abstract string TaskDefintionName { get; }

		public Type GetTaskExecutorType()
		{
			return typeof(TaskExecutor);
		}

	}
}
