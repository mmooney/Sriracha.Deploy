using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;
using MMDB.Shared;
using System.Resources;

namespace Sriracha.Deploy.Data.Impl
{
	public class TaskManager : ITaskManager
	{
		private readonly IModuleInspector _moduleInspector;

		public TaskManager(IModuleInspector moduleInspector)
		{
			this._moduleInspector = DIHelper.VerifyParameter(moduleInspector);
		}

		public List<TaskMetadata> GetAvailableTaskList()
		{
			var typeList = this._moduleInspector.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition));
			List<TaskMetadata> returnList = new List<TaskMetadata>();
			foreach(var type in typeList)
			{
				if(type.IsClass && !type.IsAbstract)
				{
                    string taskDisplayName = type.Name;
                    var attribute = type.GetCustomAttributes(typeof(TaskDefinitionMetadataAttribute), true).FirstOrDefault();
                    if(attribute != null)
                    {
                        taskDisplayName = ((TaskDefinitionMetadataAttribute)attribute).TaskName;
                    }
					var item = new TaskMetadata
					{
						TaskTypeName = type.FullName,
						TaskDisplayName = taskDisplayName
					};
					returnList.Add(item);
				}
			}
			return returnList;
		}


        public string GetTaskOptionsView(string taskTypeName)
        {
            var typeList = _moduleInspector.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition));
            var type = typeList.FirstOrDefault(i=>i.FullName == taskTypeName);
            if(type == null)
            {
                throw new ArgumentNullException("Failed to find type " + taskTypeName);
            }
            var attribute = (TaskDefinitionMetadataAttribute)type.GetCustomAttributes(typeof(TaskDefinitionMetadataAttribute), true).FirstOrDefault();
            if (attribute == null)
            {
                throw new Exception("Unable to find TaskDefinitionMetadataAttribute for type " + taskTypeName);
            }
            if(string.IsNullOrEmpty(attribute.OptionsViewResourceId))
            {
                throw new Exception("Unable to find OptionsViewResourceId for type " + taskTypeName);
            }
            var resourceNameList = type.Assembly.GetManifestResourceNames();
            foreach(var resourceName in resourceNameList)
            {
                string newResourceName = resourceName;
                if(newResourceName.EndsWith(".resources"))
                {
                    newResourceName = newResourceName.Substring(0, resourceName.Length-".resources".Length);
                }
                var resourceManager = new ResourceManager(newResourceName, type.Assembly);
                var value = resourceManager.GetString(attribute.OptionsViewResourceId);
                if(value != null)
                {
                    return value;
                }
            }
            throw new Exception("Resource " + attribute.OptionsViewResourceId + " not found in assembly " + type.Assembly.FullName);
        }
    }
}
