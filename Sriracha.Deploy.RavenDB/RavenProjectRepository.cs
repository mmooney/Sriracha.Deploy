using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenProjectRepository : IProjectRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenProjectRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public IEnumerable<DeployProject> GetProjectList(string[] idList = null)
		{
			if(idList != null && idList.Any())
			{
				return _documentSession.Query<DeployProject>().Where(i=>idList.Contains(i.Id));
			}
			else 
			{
				return _documentSession.Query<DeployProject>();
			}
		}

		public DeployProject CreateProject(string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var project = new DeployProject
			{
				Id = Guid.NewGuid().ToString(),
				ProjectName = projectName
			};
			_documentSession.Store(project);
			_documentSession.SaveChanges();
			return project;
		}

		public DeployProject GetProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			var item = TryGetProject(projectId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployProject), "Id", projectId);
			}
			return item;
		}

		public DeployProject TryGetProject(string projectId)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			return _documentSession.Load<DeployProject>(projectId);
		}

		public DeployProject TryGetProjectByName(string projectName)
		{
			if (string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var list = _documentSession.Query<DeployProject>()
											.Customize(i=>i.WaitForNonStaleResultsAsOfLastWrite())
											.Where(i=>i.ProjectName == projectName).ToList();
			if(list.Count == 0)
			{
				return null;
			}
			else if (list.Count == 1)
			{
				return list[0];
			}
			else 
			{
				throw new Exception("Multiple projects found with name " + projectName);
			}
		}

		public DeployProject GetProjectByName(string projectName)
		{
			if (string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var item = this.TryGetProjectByName(projectName);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployProject), "ProjectName", projectName);
			}
			return item;
		}

		public void DeleteProject(string projectId)
		{
			var item = this.GetProject(projectId);
			this._documentSession.Delete(item);
			this._documentSession.SaveChanges();
		}


		public DeployProject UpdateProject(string projectId, string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var item = this.GetProject(projectId);
			item.ProjectName = projectName;
			this._documentSession.SaveChanges();
			return item;
		}


		public IEnumerable<DeployComponent> GetComponentList(string projectId)
		{
			var project = GetProject(projectId);
			return project.ComponentList;
		}

		public DeployComponent CreateComponent(string projectId, string componentName)
		{
			var project = GetProject(projectId);
			var item = new DeployComponent
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ComponentName = componentName
			};
			project.ComponentList.Add(item);
			this._documentSession.SaveChanges();
			return item;
		}

		public DeployComponent GetComponent(string componentId)
		{
			if(string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			var item = this.TryGetComponent(componentId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
			}
			return item;
		}

		public DeployComponent TryGetComponent(string componentId)
		{
			var allProjects = _documentSession.Query<DeployProject>().ToList();
			var x = allProjects.FirstOrDefault(i=>i.ComponentList.Any(j=>j.Id == componentId));
			var project = _documentSession.Query<DeployProject>()
								.Customize(i=>i.WaitForNonStaleResultsAsOfLastWrite())
								.ToList()
								.FirstOrDefault(i=>i.ComponentList.Any(j=>j.Id == componentId));
			if(project == null)
			{
				return null;
			}
			else 
			{
				return project.ComponentList.FirstOrDefault(i=>i.Id == componentId);
			}
		}

		public DeployComponent TryGetComponent(DeployProject project, string componentId)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			return project.ComponentList.FirstOrDefault(i=>i.Id == componentId);
		}

		public DeployComponent GetComponentByName(DeployProject project, string componentName)
		{
			if(project == null)
			{
				throw new ArgumentNullException("Missing project");
			}
			if(string.IsNullOrWhiteSpace(componentName))
			{
				throw new ArgumentNullException("Missing component name");
			}
			var item = this.TryGetComponentByName(project, componentName);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployComponent), "ComponentName", componentName);
			}
			return item;
		}

		public DeployComponent TryGetComponentByName(DeployProject project, string componentName)
		{
			return project.ComponentList.FirstOrDefault(i=>i.ComponentName == componentName);
		}

		public DeployComponent GetComponent(DeployProject project, string componentId)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing component ID");
			}
			if (project == null)
			{
				throw new ArgumentNullException("Project is null");
			}
			var component = project.ComponentList.FirstOrDefault(i => i.Id == componentId);
			if (componentId == null)
			{
				throw new ArgumentException("Unable to find component " + componentId + " in project " + project.Id);
			}
			return component;	
		}

		public DeployComponent UpdateComponent(string componentId, string projectId, string componentName)
		{
			var project = GetProject(projectId);
			var item = project.ComponentList.Single(i=>i.Id == componentId);
			item.ComponentName = componentName;
			this._documentSession.SaveChanges();
			return item;
		}

		public void DeleteComponent(string componentId)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.Id == componentId));
			if (project == null)
			{
				throw new KeyNotFoundException("No project found for component ID " + componentId);
			}
			var component = project.ComponentList.First(i => i.Id == componentId);
			project.ComponentList.Remove(component);
			this._documentSession.SaveChanges();
		}

		public List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId)
		{
			var component = GetComponent(componentId);
			return component.DeploymentStepList;
		}

		public DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson) 
		{
			var project = GetProject(projectId);
			var component = project.ComponentList.Single(i => i.Id == componentId);
			var item = new DeployComponentDeploymentStep
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ComponentId = componentId,
				StepName = stepName,
				TaskTypeName = taskTypeName,
				TaskOptionsJson = taskOptionsJson
			};
			
			if(component.DeploymentStepList == null)
			{
				component.DeploymentStepList = new List<DeployComponentDeploymentStep>();
			}
			component.DeploymentStepList.Add(item);
			this._documentSession.SaveChanges();
			return item;
		}

		public DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId)
		{
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i=>i.ComponentList.Any(j=>j.DeploymentStepList.Any(k=>k.Id == deploymentStepId)));
			if(project == null)
			{
				throw new KeyNotFoundException("Could not find project for deployment step " + deploymentStepId);
			}
			var item = project.ComponentList.Single(i=>i.DeploymentStepList.Any(j=>j.Id == deploymentStepId)).DeploymentStepList.First(i=>i.Id == deploymentStepId);
			return item;
		}

		public DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson) 
		{
			var project = GetProject(projectId);
			var component = project.ComponentList.Single(i => i.Id == componentId);
			var item = component.DeploymentStepList.Single(i=>i.Id == deploymentStepId);
			item.StepName = stepName;
			item.TaskTypeName = taskTypeName;
			item.TaskOptionsJson = taskOptionsJson;
			this._documentSession.SaveChanges();
			return item;
		}


		public void DeleteDeploymentStep(string deploymentStepId)
		{
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.DeploymentStepList.Any(k => k.Id == deploymentStepId)));
			if (project == null)
			{
				throw new KeyNotFoundException("Could not find project for deployment step " + deploymentStepId);
			}
			var component = project.ComponentList.Single(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
			var item = component.DeploymentStepList.First(i => i.Id == deploymentStepId);
			component.DeploymentStepList.Remove(item);
			this._documentSession.SaveChanges();

		}


		public IEnumerable<DeployProjectBranch> GetBranchList(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			var project = GetProject(projectId);
			return project.BranchList;
		}

		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			if (string.IsNullOrEmpty(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			var project = this.GetProject(projectId);
			var branch = new DeployProjectBranch
			{
				Id = Guid.NewGuid().ToString(),
				BranchName = branchName,
				ProjectId = projectId
			};
			project.BranchList.Add(branch);
			this._documentSession.SaveChanges();
			return branch;
		}


		public DeployProjectBranch GetBranch(string branchId)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var branch = this.TryGetBranch(branchId);
			if(branch == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
			}
			return branch;
		}

		public DeployProjectBranch TryGetBranch(string branchId)
		{
			if (string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i => i.BranchList.Any(j => j.Id == branchId));
			if (project != null)
			{
				return project.BranchList.First(i => i.Id == branchId);
			}
			else 
			{
				return null;
			}
		}

		public DeployProjectBranch GetBranch(DeployProject project, string branchId)
		{
			if (string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			if (project == null)
			{
				throw new ArgumentNullException("Project is null");
			}
			var branch = this.TryGetBranch(project, branchId);
			if (branch == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
			}
			return branch;
		}
		
		public DeployProjectBranch TryGetBranch(DeployProject project, string branchId)
		{
			if (string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			if (project == null)
			{
				throw new ArgumentNullException("Project is null");
			}
			return project.BranchList.FirstOrDefault(i => i.Id == branchId);
		}

		public DeployProjectBranch GetBranchByName(string projectId, string branchName)
		{
			if(string.IsNullOrWhiteSpace(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			if(string.IsNullOrWhiteSpace(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			var item = this.TryGetBranchByName(projectId, branchName);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "ProjectId/BranchName", projectId + "/" + branchName);
			}
			return item;
		}

		public DeployProjectBranch TryGetBranchByName(string projectId, string branchName)
		{
			var project = this.GetProject(projectId);
			return this.GetBranchByName(project, branchName);
		}

		public DeployProjectBranch GetBranchByName(DeployProject project, string branchName)
		{
			if (project == null)
			{
				throw new ArgumentNullException("Missing Project");
			}
			if (string.IsNullOrWhiteSpace(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			return project.BranchList.FirstOrDefault(i => i.BranchName == branchName);
		}

		public DeployProjectBranch TryGetBranchByName(DeployProject project, string branchName)
		{
			return project.BranchList.FirstOrDefault(i=>i.BranchName == branchName);
		}

		public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
		{
			if(string.IsNullOrEmpty(branchId)) 
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			if(string.IsNullOrEmpty(branchName)) 
			{
				throw new ArgumentNullException("Missing branch name");
			}
			var project = GetProject(projectId);
			var branch = project.BranchList.FirstOrDefault(i=>i.Id == branchId);
			if(branchId == null)
			{
				throw new ArgumentException("Unable to find branch " + branchId + " in project " + projectId);
			}
			branch.BranchName = branchName;
			this._documentSession.SaveChanges();
			return branch;
		}

		public void DeleteBranch(string branchId)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i=>i.BranchList.Any(j=>j.Id == branchId));
			if(project == null)
			{
				throw new ArgumentException("Unable to find project for branch ID " + branchId);
			}
			var branch = project.BranchList.First(i=>i.Id == branchId);
			project.BranchList.Remove(branch);
			this._documentSession.SaveChanges();
		}


		public IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			var project = GetProject(projectId);
			return project.EnvironmentList;
		}

		public DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			if(string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException("Missing environment name");
			}
			var project = GetProject(projectId);
			var environment = new DeployEnvironment
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				EnvironmentName = environmentName,
				ComponentList = componentList.ToList()
			};
			UpdateComponentList(componentList, project, environment);
			if(project.EnvironmentList == null)
			{
				project.EnvironmentList = new List<DeployEnvironment>();
			}
			project.EnvironmentList.Add(environment);
			this._documentSession.SaveChanges();
			return environment;
		}

		private static void UpdateComponentList(IEnumerable<DeployEnvironmentComponent> componentList, DeployProject project, DeployEnvironment environment)
		{
			foreach (var component in componentList)
			{
				if (string.IsNullOrEmpty(component.Id))
				{
					component.Id = Guid.NewGuid().ToString();
				}
				component.EnvironmentId = environment.Id;
				component.ProjectId = project.Id;

				if (component.MachineList != null)
				{
					foreach (var machine in component.MachineList)
					{
						if (string.IsNullOrEmpty(machine.Id))
						{
							machine.Id = Guid.NewGuid().ToString();
						}
						machine.ProjectId = project.Id;
						machine.EnvironmentComponentId = component.Id;
						machine.EnvironmentId = environment.Id;
					}
				}
			}
		}

		public DeployEnvironment GetEnvironment(string environmentId)
		{
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
			var project = this._documentSession.Query<DeployProject>()
							.ToList()
							.FirstOrDefault(i=>i.EnvironmentList.Any(j=>j.Id == environmentId));
			if(project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i=>i.Id == environmentId);
			return environment;
		}

		public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
			if (string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException("Missing environment name");
			}
			var project = this._documentSession.Query<DeployProject>()
								.ToList()
								.FirstOrDefault(i => i.EnvironmentList.Any(j => j.Id == environmentId));
			if (project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i => i.Id == environmentId);
			environment.EnvironmentName = environmentName;
			environment.ComponentList = componentList.ToList();
			UpdateComponentList(componentList, project, environment);
			this._documentSession.SaveChanges();
			return environment;
		}

		public void DeleteEnvironment(string environmentId)
		{
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
			var project = this._documentSession.Query<DeployProject>()
									.ToList()
									.FirstOrDefault(i => i.EnvironmentList.Any(j => j.Id == environmentId));
			if (project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i => i.Id == environmentId);
			project.EnvironmentList.Remove(environment);
			this._documentSession.SaveChanges();
		}


		public DeployMachine GetMachine(string machineId)
		{
			if (string.IsNullOrEmpty(machineId))
			{
				throw new ArgumentNullException("Missing machine ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i => i.EnvironmentList.Any(j => j.ComponentList.Any(k=>k.MachineList.Any(l=>l.Id == machineId))));
			if (project == null)
			{
				throw new ArgumentException("Unable to find project for machine ID " + machineId);
			}
			return project.GetMachine(machineId);
		}

		public DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configValueList)
		{
			var project = GetProject(projectId);
			var item = project.GetMachine(machineId);
			item.MachineName = machineId;
			item.ConfigurationValueList = configValueList;
			this._documentSession.SaveChanges();
			return item;
		}
	}
}
