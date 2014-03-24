using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public static class DIHelper
	{
        //Eventually this will do something more meaningful/informative
		public static T VerifyParameter<T>(T parameterValue) where T:class 
		{
			if(parameterValue == null)
			{
				throw new Exception(typeof(T).FullName + " cannot be null");
			}
			return parameterValue;
		}
	}
}
