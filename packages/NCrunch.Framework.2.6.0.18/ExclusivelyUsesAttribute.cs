using System;

namespace NCrunch.Framework
{
	public class ExclusivelyUsesAttribute: ResourceUsageAttribute
	{
		public ExclusivelyUsesAttribute(params string[] resourceName) : base(resourceName) {}
	}
}
