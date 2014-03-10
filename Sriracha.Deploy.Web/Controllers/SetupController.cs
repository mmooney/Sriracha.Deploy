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
        private readonly ISystemSettings _systemSettings;

        public SetupController(ISystemSetterUpper systemSetterUpper, ISystemSettings systemSettings)
        {
            _systemSetterUpper = DIHelper.VerifyParameter(systemSetterUpper);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
        }

        public ActionResult Index()
        {
            if (_systemSettings.IsInitialized())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Start()
        {
            if(_systemSettings.IsInitialized())
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new SystemSetupData
            {
                AdministratorUserName = "administrator"
            };
            var existingAdminUser = _systemSetterUpper.GetAdministratorUser();
            if(existingAdminUser != null)
            {
                model.AdministratorUserName = existingAdminUser.UserName;
                model.AdministratorEmailAddress = existingAdminUser.EmailAddress;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Start(SystemSetupData model)
        {
            if (_systemSettings.IsInitialized())
            {
                return RedirectToAction("Index", "Home");
            }
            if (this.ModelState.IsValid)
            {
                try 
                {
                    _systemSetterUpper.SetupAdministratorUser(model.AdministratorUserName, model.AdministratorPassword, model.AdministratorEmailAddress);
                    return RedirectToAction("Configure", "Setup");
                }
                catch(Exception err)
                {
                    this.ModelState.AddModelError("", err);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);

        }

        public ActionResult Configure()
        {
            if (_systemSettings.IsInitialized())
            {
                return RedirectToAction("Index", "Home");
            }
            var existingAdminUser = _systemSetterUpper.GetAdministratorUser();
            if (existingAdminUser == null)
            {
                return RedirectToAction("Start", "Setup");
            }
            var model = new SystemConfigureData
            {
                AllowSelfRegistration = true,
                DefaultAccess = EnumPermissionAccess.Grant
            };
            if(_systemSettings.IsInitialized())
            {
                model.AllowSelfRegistration = _systemSettings.AllowSelfRegistration;
                model.DefaultAccess = _systemSettings.DefaultAccess;
            }
            return View(model);            
        }
        
        [HttpPost]
        public ActionResult Configure(SystemConfigureData model)
        {
            if (_systemSettings.IsInitialized())
            {
                return RedirectToAction("Index", "Home");
            }
            var existingAdminUser = _systemSetterUpper.GetAdministratorUser();
            if (existingAdminUser == null)
            {
                return RedirectToAction("Start", "Setup");
            }
            if (this.ModelState.IsValid)
            {
                try 
                {
                    _systemSetterUpper.SetupAdministratorUser(model.AllowSelfRegistration, model.DefaultAccess.GetValueOrDefault());
                    return RedirectToAction("Complete", "Setup");
                }
                catch(Exception err)
                {
                    this.ModelState.AddModelError("", err);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Complete()
        {
            return View();
        }
    }
}
