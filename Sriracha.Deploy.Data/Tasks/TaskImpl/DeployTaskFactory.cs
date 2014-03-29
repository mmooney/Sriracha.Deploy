using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sriracha.Deploy.Data.Tasks.TaskImpl
{
	public class DeployTaskFactory : IDeployTaskFactory
	{
		public readonly IDIFactory _diFactory;
		public IModuleInspector _moduleInspector;

		public DeployTaskFactory(IDIFactory diFactory, IModuleInspector moduleInspector)
		{
			_diFactory = DIHelper.VerifyParameter(diFactory);
			_moduleInspector = DIHelper.VerifyParameter(moduleInspector);
		}
		public IDeployTaskExecutor CreateTaskExecutor(Type t)
		{
			return (IDeployTaskExecutor)_diFactory.CreateInjectedObject(t);
		}

		public IDeployTaskDefinition CreateTaskDefinition(string taskTypeName, string taskOptionJson)
		{
			var t = _moduleInspector.GetType(taskTypeName);
			var returnValue = (IDeployTaskDefinition)_diFactory.CreateInjectedObject(t);
			returnValue.DeployTaskOptions = JsonConvert.DeserializeObject(taskOptionJson, returnValue.GetTaskOptionType());
			return returnValue;
		}
	}
}
