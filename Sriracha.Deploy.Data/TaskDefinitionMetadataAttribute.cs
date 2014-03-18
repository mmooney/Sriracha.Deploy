using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class TaskDefinitionMetadataAttribute : Attribute 
    {
        public string TaskName { get; set; }

        public TaskDefinitionMetadataAttribute(string taskName = null)
        {
            this.TaskName = taskName;
        }
    }
}
