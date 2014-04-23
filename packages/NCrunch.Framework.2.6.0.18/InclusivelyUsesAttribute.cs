using System;

namespace NCrunch.Framework
{
	public class InclusivelyUsesAttribute : ResourceUsageAttribute
	{
		public InclusivelyUsesAttribute(params string[] resourceNames): base(resourceNames) { }
	}
}
