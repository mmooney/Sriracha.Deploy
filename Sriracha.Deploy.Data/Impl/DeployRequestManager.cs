using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployRequestManager : IDeployRequestManager
	{
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeployRepository _deployRepository;
		private readonly IDeploymentValidator _validator;

		public DeployRequestManager(IBuildRepository buildRepository, IProjectRepository projectRepository, IDeployRepository deployRepository, IDeploymentValidator validator)
		{
			_buildRepository = buildRepository;
			_projectRepository = projectRepository;
			_deployRepository = deployRepository;
			_validator = validator;
		}
		public DeployRequestTemplate InitializeDeployRequest(string buildId, string environmentId)
		{
			var returnValue = new DeployRequestTemplate
			{
				Build = _buildRepository.GetBuild(buildId),
				Environment = _projectRepository.GetEnvironment(environmentId),
			};
			returnValue.Component = _projectRepository.GetComponent(returnValue.Build.ProjectComponentId);
			returnValue.ValidationResult = _validator.ValidateDeployment(returnValue.Component, returnValue.Environment);
			return returnValue;
		}


		public DeployState SubmitDeployRequest(string projectId, string buildId, string environmentId, IEnumerable<string> machineIdList)
		{
			var build = _buildRepository.GetBuild(buildId);
			var environment = _projectRepository.GetEnvironment(environmentId);
			var component = _projectRepository.GetComponent(build.ProjectComponentId);
			var branch = _projectRepository.GetBranch(build.ProjectBranchId);
			var validationResult = _validator.ValidateDeployment(component, environment);
			var machineList = new List<DeployMachine>();
			foreach(string id in machineIdList)
			{
				var machine = _projectRepository.GetMachine(id);
				machineList.Add(machine);
			}
			return  _deployRepository.CreateDeployment(build, branch, environment, component, machineList, null);
		}


		public List<DeployBatchRequest> GetDeployBatchRequestList()
		{
			return _deployRepository.GetBatchRequestList();
		}

		public DeployBatchRequest GetDeployBatchRequest(string id)
		{
			return _deployRepository.GetBatchRequest(id);
		}


		public DeployBatchRequest CreateDeployBatchRequest(List<DeployBatchRequestItem> itemList)
		{
			return _deployRepository.CreateBatchRequest(itemList, DateTime.UtcNow);
		}

		public PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions)
		{
			return _deployRepository.GetDeployBatchStatusList(listOptions);
		}

		public DeployBatchStatus GetDeployBatchStatus(string deployBatchRequestId)
		{
			var status = new DeployBatchStatus
			{
				DeployBatchRequestId = deployBatchRequestId,
				Request = _deployRepository.GetBatchRequest(deployBatchRequestId),
				DeployStateList = new List<DeployStateSummary>()
			};
			foreach(var requestItem in status.Request.ItemList)
			{
				var state = _deployRepository.TryGetDeployStateSummaryByDeployBatchRequestItemId(deployBatchRequestId);
				if(state != null)
				{
					status.DeployStateList.Add(state);
				}
			}
			return status;
		}
	}
}
