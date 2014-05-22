using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class RazorTemplate : BaseDto
	{
		public string ViewName { get; set; }
		public string ViewData { get; set; }
	}
}
