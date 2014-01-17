using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Account;
using Sriracha.Deploy.Data.SystemSettings;

namespace Sriracha.Deploy.Web.Controllers
{
    public class HomeController : Controller
    {
		private readonly IAccountSettingsManager _accountSettingsManager;
        private readonly ISystemSettings _systemSettings;

		public HomeController(IAccountSettingsManager accountSettingsManager, ISystemSettings systemSettings)
		{
			_accountSettingsManager = DIHelper.VerifyParameter(accountSettingsManager);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
        }

        public ActionResult Index()
        {
            if(!_systemSettings.IsInitialized())
            {
                return RedirectToAction("Index","Setup");
            }
			if (this.User != null && this.User.Identity != null && this.User.Identity.IsAuthenticated)
			{
				if(this.User is WindowsPrincipal)
				{
					_accountSettingsManager.EnsureUserAccount(this.User.Identity.Name);
				}
			}
			return View();
        }

		public Action TestError()
		{
			throw new Exception("Test Error");
		}

    }
}
