using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class SrirachaEmailMessage
	{
		public string Id { get; set; }
		public List<string> EmailAddresseList { get; set; }
		public object DataObject { get; set; }
		public string RazorView { get; set; }
	}
}
