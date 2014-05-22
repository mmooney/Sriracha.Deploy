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
        private readonly IUserIdentity _userIdentity;

		public RavenRazorTemplateRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
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
            if(string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException("viewName");
            }
			return "RazorTemplate_" + viewName.Replace('\\','_'); 
		}


        public RazorTemplate SaveTemplate(string viewName, string viewData)
        {
            if(string.IsNullOrEmpty(viewData))
            {
                throw new ArgumentNullException("viewData");
            }
            var template = _documentSession.Load<RazorTemplate>(FormatId(viewName));
            if(template == null)
            {
                template = new RazorTemplate
                {
                    Id = FormatId(viewName),
                    ViewName = viewName,
                    ViewData = viewData 
                };
                template.SetCreatedFields(_userIdentity.UserName);
                return _documentSession.StoreSaveEvict(template);
            }
            else 
            {
                template.ViewData = viewData;
                template.SetUpdatedFields(_userIdentity.UserName);
                return _documentSession.SaveEvict(template);
            }
        }

        public RazorTemplate DeleteTemplate(string viewName)
        {
            var item = _documentSession.LoadEnsure<RazorTemplate>(FormatId(viewName));
            return _documentSession.DeleteSaveEvict(item);
        }
    }
}
