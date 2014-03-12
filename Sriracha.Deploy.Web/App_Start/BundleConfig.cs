using System.Web;
using System.Web.Optimization;

namespace Sriracha.Deploy.Web
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
            bundles.Add(new ScriptBundle("~/vendor/underscore/scripts")
                            .Include("~/vendor/underscore/*.js"));

            bundles.Add(new ScriptBundle("~/vendor/bootstrap/scripts")
                                .Include("~/vendor/bootstrap/*.js"));

            bundles.Add(new StyleBundle("~/vendor/bootstrap/styles")
                            .Include("~/vendor/bootstrap/bootstrap.css")
                            .Include("~/vendor/bootstrap/bootstrap-responsive.css")
            );
            
            bundles.Add(new ScriptBundle("~/vendor/angular/scripts")
                        .Include("~/vendor/angular/angular.js")
                        .Include("~/vendor/angular/angular-resource.js")
                        .Include("~/vendor/angular/angular-route.js")
                        .Include("~/vendor/ng-upload/ng-upload.js")
            );

            bundles.Add(new ScriptBundle("~/vendor/angular-ui-bootstrap/scripts")
                        .IncludeDirectory("~/vendor/angular-ui-bootstrap", "*.js"));

			var srirachaBundleJS = new ScriptBundle("~/app/sriracha/scripts")
										.Include("~/app/shared.js")
                                        .Include("~/app/ngSriracha.js")
                                        .Include("~/app/resources.js")
                                        .Include("~/app/navigator.js")
                                        .Include("~/app/router.js")
                                        .IncludeDirectory("~/app", "*.js", true);
			bundles.Add(srirachaBundleJS);

			var srirachaBundleCSS = new StyleBundle("~/app/sriracha/styles")
										.Include("~/app/sriracha.css");
			bundles.Add(srirachaBundleCSS);

            bundles.Add(new ScriptBundle("~/vendor/jquery/scripts")
                .Include("~/vendor/jquery/jquery-{version}.js")
                .Include("~/vendor/jquery/jquery-ui-{version}.js")
                .Include("~/vendor/jquery/jquery.unobtrusive*")
                .Include("~/vendor/jquery/jquery.validate*")
                .Include("~/vendor/jquery/jquery.blockUI.js")
            );
            
            bundles.Add(new StyleBundle("~/vendor/jquery/styles").Include(
						"~/vendor/jquery/themes/base/jquery.ui.core.css",
                        "~/vendor/jquery/themes/base/jquery.ui.resizable.css",
                        "~/vendor/jquery/themes/base/jquery.ui.selectable.css",
						"~/vendor/jquery/themes/base/jquery.ui.accordion.css",
						"~/vendor/jquery/themes/base/jquery.ui.autocomplete.css",
						"~/vendor/jquery/themes/base/jquery.ui.button.css",
						"~/vendor/jquery/themes/base/jquery.ui.dialog.css",
						"~/vendor/jquery/themes/base/jquery.ui.slider.css",
						"~/vendor/jquery/themes/base/jquery.ui.tabs.css",
						"~/vendor/jquery/themes/base/jquery.ui.datepicker.css",
						"~/vendor/jquery/themes/base/jquery.ui.progressbar.css",
						"~/vendor/jquery/themes/base/jquery.ui.theme.css")
            );
		}
	}
}