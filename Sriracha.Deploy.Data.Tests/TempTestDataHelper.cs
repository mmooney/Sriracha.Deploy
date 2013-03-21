using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests
{
	[Obsolete("Move into MMDB.Shared")]
	public static class TempTestDataHelper
	{
		public static byte[] RandomBytes(int size)
		{
			var data = new byte[size];
			TestDataHelper.Random.NextBytes(data);
			return data;
		}
	}
}
