using System;

namespace NCrunch.Framework
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
	public class RequiresCapabilityAttribute: Attribute
	{
		public RequiresCapabilityAttribute(string capabilityName)
		{
			CapabilityName = capabilityName;
		}

		public string CapabilityName { get; private set; }
	}
}
