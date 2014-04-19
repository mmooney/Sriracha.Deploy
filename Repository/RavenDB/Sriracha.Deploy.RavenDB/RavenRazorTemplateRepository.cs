using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenRazorTemplateRepository : IRazorTemplateRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenRazorTemplateRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public RazorTemplate GetTemplate(string viewName, string defaultViewData)
		{
			var item = _documentSession.Load<RazorTemplate>(FormatId(viewName));
			if(item == null)
			{
				item = new RazorTemplate
				{
					ViewName = viewName,
					ViewData = defaultViewData
				};
			};
			return item;
		}

		private string FormatId(string viewName)
		{
			return "RazorTemplate_" + viewName.Replace('\\','_'); 
		}
	}
}
