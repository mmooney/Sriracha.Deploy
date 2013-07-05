using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployEnvironment
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentName { get; set; }
		public List<DeployEnvironmentComponent> ComponentList { get; set; }

		public DeployEnvironment()
		{
			this.ComponentList = new List<DeployEnvironmentComponent>();
		}

		public DeployEnvironmentComponent GetEnvironmentComponent(string componentId)
		{
			if(this.ComponentList == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentComponent), "ComponentId", componentId);
			}
			var returnValue = this.TryGetEnvironmentComponent(componentId);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployEnvironmentComponent), "ComponentId", componentId);
			}
			return returnValue;
		}

		private DeployEnvironmentComponent TryGetEnvironmentComponent(string componentId)
		{
			if(this.ComponentList != null)
			{
				return this.ComponentList.SingleOrDefault(i=>i.ComponentId == componentId);
			}
			else 
			{
				return null;
			}
		}
	}
}
