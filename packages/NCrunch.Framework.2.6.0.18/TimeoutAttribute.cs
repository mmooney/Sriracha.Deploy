using System.Collections;

namespace NCrunch.Framework
{
	public class TimeoutAttribute: System.Attribute
	{
		private IDictionary _properties;

		public TimeoutAttribute(int timeout)
		{
			_properties = new Hashtable();
			_properties["Timeout"] = timeout;
		}

		public IDictionary Properties
		{
			get { return _properties; }
		}
	}
}
