using System.Web;
using System.Web.Optimization;

namespace Sriracha.Deploy.Web
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/js")
				.Include("~/Scripts/jquery/jquery-{version}.js")
				.Include("~/Scripts/jquery/jquery-ui-{version}.js")
				.Include("~/Scripts/jquery/jquery.unobtrusive*")
				.Include("~/Scripts/jquery/jquery.validate*")
				.Include("~/Scripts/jquery/jquery.blockUI.js")
				.Include("~/Scripts/underscore.js")
				.Include("~/Scripts/backbone.js")
			);

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			var srirachaBundle = new ScriptBundle("~/bundles/sriracha")
										.Include("~/scripts/sriracha/shared.js")
										.Include("~/scripts/sriracha/ngSriracha.js")
										.Include("~/scripts/sriracha/resources.js")
										.Include("~/scripts/sriracha/navigator.js")
										.Include("~/scripts/sriracha/router.js")
										.IncludeDirectory("~/scripts/sriracha", "*.js", true);
			bundles.Add(srirachaBundle);

			//bundles.Add(new StyleBundle("~/Content/css")
			//			.Include("~/Content/site.css")
			//			.Include("~/Content/bootstrap.css")
			//			.Include("~/Content/bootstrap-responsive.css"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
						"~/Content/themes/base/jquery.ui.core.css",
						"~/Content/themes/base/jquery.ui.resizable.css",
						"~/Content/themes/base/jquery.ui.selectable.css",
						"~/Content/themes/base/jquery.ui.accordion.css",
						"~/Content/themes/base/jquery.ui.autocomplete.css",
						"~/Content/themes/base/jquery.ui.button.css",
						"~/Content/themes/base/jquery.ui.dialog.css",
						"~/Content/themes/base/jquery.ui.slider.css",
						"~/Content/themes/base/jquery.ui.tabs.css",
						"~/Content/themes/base/jquery.ui.datepicker.css",
						"~/Content/themes/base/jquery.ui.progressbar.css",
						"~/Content/themes/base/jquery.ui.theme.css"));
		}
	}
}