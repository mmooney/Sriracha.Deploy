using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IRazorTemplateRepository
	{
		RazorTemplate GetTemplate(string viewName, string defaultViewData);
	}
}
