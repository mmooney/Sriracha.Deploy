using System;

namespace NCrunch.Framework
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Assembly, AllowMultiple = true)]
	public class CategoryAttribute: Attribute
	{
		public CategoryAttribute(string category)
		{
			Category = category;
		}

		public string Category { get; private set; }
	}
}
