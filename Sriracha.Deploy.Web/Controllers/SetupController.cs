using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.SystemSettings;
using Sriracha.Deploy.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sriracha.Deploy.Web.Controllers
{
    public class SetupController : Controller
    {
        //
        // GET: /Setup/

        private readonly ISystemSetterUpper _systemSetterUpper;

        public SetupController(ISystemSetterUpper systemSetterUpper)
        {
            _systemSetterUpper = DIHelper.VerifyParameter(systemSetterUpper);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Start()
        {
            var model = new SystemSetupData
            {
                AdministratorUserName = "administrator"
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Start(SystemSetupData model)
        {
            if (this.ModelState.IsValid)
            {
                try 
                {
                    _systemSetterUpper.SetupSystem(model.AdministratorUserName, model.AdministratorPassword, model.AdministratorEmailAddress);
                    return RedirectToAction("Index", "Home");
                }
                catch(Exception err)
                {
                    this.ModelState.AddModelError("", err);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);

        }

    }
}
