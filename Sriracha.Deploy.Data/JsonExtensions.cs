using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace System
{
	public static class JsonExtensions
	{
		public static string ToJson(this object thisObject)
		{
			return JsonConvert.SerializeObject(thisObject);
		}

        public static T FromJson<T>(this string thisString)
        {
            return JsonConvert.DeserializeObject<T>(thisString);
        }
	}
}
