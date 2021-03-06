﻿using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Sriracha.Deploy.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public MvcApplication()
		{
			this.AuthenticateRequest += MvcApplication_AuthenticateRequest;
            this.BeginRequest += MvcApplication_BeginRequest;
		}

        void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            if(HttpContext.Current != null && !HttpContext.Current.Request.IsLocal 
                && !HttpContext.Current.Request.IsSecureConnection && AppSettingsHelper.GetBoolSetting("RequireSSL", true))
            {
                string redirectUrl = HttpContext.Current.Request.Url.ToString().Replace("http://", "https://");
                HttpContext.Current.Response.Redirect(redirectUrl, false);
                this.CompleteRequest();
            }
        }

		void MvcApplication_AuthenticateRequest(object sender, EventArgs e)
		{
		}


		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

	}
}