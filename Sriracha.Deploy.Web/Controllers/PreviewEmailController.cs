using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Web.Controllers
{
    public class PreviewEmailController : Controller
    {
		private readonly IDeployRepository _deployRepository;
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly ISystemSettings _systemSettings;
		private readonly IUrlGenerator _urlGenerator;
		private readonly IRazorTemplateRepository _razorTemplateRepository;

		public PreviewEmailController(IDeployRepository deployRepository, IBuildRepository buildRepository, IProjectRepository projectRepository, ISystemSettings systemSettings, IUrlGenerator urlGenerator, IRazorTemplateRepository razorTemplateRepository)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_urlGenerator = DIHelper.VerifyParameter(urlGenerator);
			_razorTemplateRepository = DIHelper.VerifyParameter(razorTemplateRepository);
		}
        //
        // GET: /PreviewEmail/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult BuildPublished(string buildId)
		{
			try 
			{
				var build = _buildRepository.GetBuild(buildId);
				var project = _projectRepository.GetProject(build.ProjectId);
				var dataObject = new
				{
					Project = project,
					Build = build,
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					ViewBuildUrl = _urlGenerator.ViewBuildUrl(build.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("BuildPublishEmail", SrirachaResources.BuildPublishEmailView);
				var output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch(RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>",err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}


		public ActionResult DeployRequested(string deployBatchRequestId)
		{
			try 
			{
				var deployRequest = _deployRepository.GetBatchRequest(deployBatchRequestId);
				var dataObject = new
				{
					DeployRequest = deployRequest,
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
			
				};
				var template = _razorTemplateRepository.GetTemplate("DeployRequestedEmail", SrirachaResources.DeployRequestedEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch(RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>",err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}
    }
}
