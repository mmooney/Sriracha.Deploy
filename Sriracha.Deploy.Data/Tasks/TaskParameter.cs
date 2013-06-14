using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
	public enum EnumTaskParameterType
	{
		String,
		Integer,
		Decimal,
		Date,
		Enumeration
	}

	public class TaskParameter
	{
		public string FieldName { get; set; }
		public EnumTaskParameterType FieldType { get; set; }
	}
}
