using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Web.Controllers
{
    public class PreviewEmailController : Controller
    {
		private readonly IDeployRepository _deployRepository;
        private readonly IDeployStateRepository _deployStateRepository;
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly ISystemSettings _systemSettings;
		private readonly IUrlGenerator _urlGenerator;
		private readonly IRazorTemplateRepository _razorTemplateRepository;
		private readonly INotificationResourceViews _notificationResourceViews;

		public PreviewEmailController(IDeployRepository deployRepository, IDeployStateRepository deployStateRepository,  IBuildRepository buildRepository, IProjectRepository projectRepository, ISystemSettings systemSettings, IUrlGenerator urlGenerator, IRazorTemplateRepository razorTemplateRepository, INotificationResourceViews notificationResourceViews)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
            _deployStateRepository = DIHelper.VerifyParameter(deployStateRepository);
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_urlGenerator = DIHelper.VerifyParameter(urlGenerator);
			_razorTemplateRepository = DIHelper.VerifyParameter(razorTemplateRepository);
			_notificationResourceViews = DIHelper.VerifyParameter(notificationResourceViews);
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
				var template = _razorTemplateRepository.GetTemplate("BuildPublishEmail", _notificationResourceViews.BuildPublishEmailView);
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
				var template = _razorTemplateRepository.GetTemplate("DeployRequestedEmail", _notificationResourceViews.DeployRequestedEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch(RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>",err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}

		public ActionResult DeployApproved(string deployBatchRequestId)
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
				var template = _razorTemplateRepository.GetTemplate("DeployApprovedEmail", _notificationResourceViews.DeployApprovedEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch (RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>", err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}

		public ActionResult DeployRejected(string deployBatchRequestId)
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
				var template = _razorTemplateRepository.GetTemplate("DeployRejectedEmail", _notificationResourceViews.DeployRejectedEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch (RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>", err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}

		public ActionResult DeployStarted(string deployBatchRequestId)
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
				var template = _razorTemplateRepository.GetTemplate("DeployStartedEmail", _notificationResourceViews.DeployStartedEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch (RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>", err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}

		public ActionResult DeploySuccess(string deployBatchRequestId)
		{
			try
			{
				var deployRequest = _deployRepository.GetBatchRequest(deployBatchRequestId);
				var dataObject = new
				{
					DeployBatchStatus = new DeployBatchStatus
					{
						DeployBatchRequestId = deployRequest.Id,
						Request = _deployRepository.GetBatchRequest(deployRequest.Id),
						DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployRequest.Id)
					},
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeploySuccessEmail", _notificationResourceViews.DeploySuccessEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch (RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>", err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}

		public ActionResult DeployFailed(string deployBatchRequestId)
		{
			try
			{
				var deployRequest = _deployRepository.GetBatchRequest(deployBatchRequestId);
				var dataObject = new
				{
					DeployBatchStatus = new DeployBatchStatus
					{
						DeployBatchRequestId = deployRequest.Id,
						Request = _deployRepository.GetBatchRequest(deployRequest.Id),
						DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployRequest.Id)
					},
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployFailedEmail", _notificationResourceViews.DeployFailedEmailView);
				string output = RazorEngine.Razor.Parse(template.ViewData, dataObject);
				return View("ViewEmail", (object)output);
			}
			catch (RazorEngine.Templating.TemplateCompilationException err)
			{
				string output = "<div style='color:red'>ERROR: " + string.Join("<br/>", err.Errors) + "</div>";
				return View("ViewEmail", (object)output);
			}
		}
	}
}
