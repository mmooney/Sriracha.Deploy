using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MMDB.Shared;
using NLog;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenProjectRepository : IProjectRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;
		private readonly Logger _logger;

		public RavenProjectRepository(IDocumentSession documentSession, IUserIdentity userIdentity, Logger logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public IEnumerable<DeployProject> GetProjectList()
		{
			return _documentSession.QueryNoCache<DeployProject>().Customize(i => i.NoCaching()).Customize(i => i.NoTracking());
		}

		public DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var project = new DeployProject
			{
				Id = Guid.NewGuid().ToString(),
				ProjectName = projectName,
				UsesSharedComponentConfiguration = usesSharedComponentConfiguration,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			return _documentSession.StoreSaveEvict(project);
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

        public DeployProject GetOrCreateProject(string projectIdOrName)
		{
			int retryCounter = 5;
			while(true)
			{
				try 
				{
					string itemId;
					using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel=IsolationLevel.Serializable} ))
					{
                        var item = _documentSession.LoadNoCache<DeployProject>(projectIdOrName);
						if(item != null)
						{
							itemId = item.Id;
						}
						else 
						{
							item = _documentSession.Query<DeployProject>()
												.Customize(i=>i.WaitForNonStaleResultsAsOfLastWrite())
												.Customize(i=>i.NoTracking()).Customize(i=>i.NoCaching())
                                                .FirstOrDefault(i => i.ProjectName == projectIdOrName);
							if(item != null)
							{
								itemId = item.Id;
							}
							else 
							{
                                item = CreateProject(projectIdOrName, false);
								itemId = item.Id;
								transaction.Complete();
							}
						}
					}
					return _documentSession.Load<DeployProject>(itemId);
				}
				catch (Raven.Abstractions.Exceptions.ConcurrencyException exception)
				{
					retryCounter--;
					if (retryCounter <= 0)
					{
						throw;
					}
					else
					{
						_logger.WarnException(string.Format("GetOrCreateProject concurrency exception, {0} retries remaining: {1}", retryCounter, exception.ToString()), exception);
					}
				}
			}
		}

		public DeployProject TryGetProject(string projectId)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			return _documentSession.LoadNoCache<DeployProject>(projectId);
		}

		public DeployProject TryGetProjectByName(string projectName)
		{
			if (string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var list = _documentSession.QueryNoCache<DeployProject>()
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
                throw new ArgumentException("Multiple projects found with name " + projectName);
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
			_logger.Info("User {0} deleting project {1}", _userIdentity.UserName, projectId);
			var item = _documentSession.LoadEnsure<DeployProject>(projectId);
			_documentSession.DeleteSaveEvict(item);
		}


		public DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var item = _documentSession.LoadEnsure<DeployProject>(projectId);
			item.ProjectName = projectName;
			item.UsesSharedComponentConfiguration = usesSharedComponentConfiguration;
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			_documentSession.SaveChanges();
			_documentSession.Advanced.Evict(item);
			return item;
		}

		public List<DeployConfiguration> GetConfigurationList(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			return project.ConfigurationList;
		}

		public DeployConfiguration GetConfiguration(string configurationId, string projectId = null)
		{
			if(string.IsNullOrEmpty(configurationId))
			{
				throw new ArgumentNullException("Missing Configuration ID");
		    }	
			var item = this.TryGetConfiguration(configurationId, projectId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
			}
			return item;
		}

        public DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
		{
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var item = new DeployConfiguration
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = project.Id,
				ConfigurationName = configurationName,
                IsolationType = isolationType,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			project.ConfigurationList.Add(item);
			_documentSession.SaveEvict(project);
			return item;
		}

        public DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
		{
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var item = project.ConfigurationList.SingleOrDefault(i=>i.Id == configurationId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
            }
			item.ConfigurationName = configurationName;
            item.IsolationType = isolationType;
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			this._documentSession.SaveEvict(project);
			return item;
		}

		public void DeleteConfiguration(string configurationId)
		{
			if (string.IsNullOrEmpty(configurationId))
			{
				throw new ArgumentNullException("Missing Configuration ID");
			}
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ConfigurationList.Any(j => j.Id == configurationId));
			if (project == null)
			{
				throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
			}
			_logger.Info("User {0} deleting configuration {1}", _userIdentity.UserName, configurationId);
			var configuration = project.ConfigurationList.First(i => i.Id == configurationId);
			project.ConfigurationList.Remove(configuration);
			_documentSession.SaveEvict(project);
		}

		public DeployConfiguration TryGetConfiguration(string configurationId, string projectId=null)
		{
            DeployProject project;
            if(!string.IsNullOrEmpty(projectId))
            {
                project = _documentSession.LoadNoCache<DeployProject>(projectId);
            }
            else 
            {
			    project = _documentSession.QueryNoCache<DeployProject>()
								    .Customize(i=>i.WaitForNonStaleResultsAsOfLastWrite())
								    .ToList()
								    .FirstOrDefault(i=>i.ConfigurationList.Any(j=>j.Id == configurationId));
            }
			if(project == null)
			{
				return null;
			}
			else 
			{
				return project.ConfigurationList.FirstOrDefault(i=>i.Id == configurationId);
			}
		}

		public List<DeployComponent> GetComponentList(string projectId)
		{
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			return project.ComponentList;
		}

        public DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
		{
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var item = new DeployComponent
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = project.Id,
				ComponentName = componentName,
				UseConfigurationGroup = useConfigurationGroup,
				ConfigurationId = configurationId,
                IsolationType = isolationType,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			project.ComponentList.Add(item);
			this._documentSession.SaveEvict(project);
			return item;
		}

		public DeployComponent GetComponent(string componentId, string projectId=null)
		{
			if(string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			var item = this.TryGetComponent(componentId, projectId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
			}
			return item;
		}

		public DeployComponent TryGetComponent(string componentId, string projectId=null)
		{
			DeployProject project;
			if(!string.IsNullOrEmpty(projectId))
			{
				project = _documentSession.LoadNoCache<DeployProject>(projectId);
			}
			else 
			{
				project = _documentSession.QueryNoCache<DeployProject>()
								.Customize(i=>i.WaitForNonStaleResultsAsOfLastWrite())
								.ToList()
								.FirstOrDefault(i=>i.ComponentList.Any(j=>j.Id == componentId));
			}
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

		public DeployComponent GetOrCreateComponent(string projectId, string componentIdOrName)
		{
            if(string.IsNullOrEmpty(componentIdOrName))
            {
                throw new ArgumentNullException("Missing component ID or name");
            }
			int retryCounter = 5;
			while(true)
			{
				try 
				{
					string itemId;
					using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel=IsolationLevel.Serializable }))
					{
						var project = GetProject(projectId);
                        var component = project.ComponentList.FirstOrDefault(i => i.Id == componentIdOrName);
						if(component != null)
						{
							itemId = component.Id;
						}
						else 
						{
                            component = project.ComponentList.FirstOrDefault(i => i.ComponentName == componentIdOrName);
							if(component != null)
							{
								itemId = component.Id;
							}
							else 
							{
                                component = CreateComponent(projectId, componentIdOrName, false, null, EnumDeploymentIsolationType.IsolatedPerMachine);
								transaction.Complete();
								itemId = component.Id;
							}
						}
					}
					return GetComponent(itemId, projectId);
				}
				catch (Raven.Abstractions.Exceptions.ConcurrencyException exception)
				{
					retryCounter--;
					if (retryCounter <= 0)
					{
						throw;
					}
					else
					{
						_logger.WarnException(string.Format("GetOrCreateComponent concurrency exception, {0} retries remaining: {1}", retryCounter, exception.ToString()), exception);
					}
				}
			}
		}

        public DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
		{
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var item = project.ComponentList.SingleOrDefault(i=>i.Id == componentId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
            }
			item.ComponentName = componentName;
			item.UseConfigurationGroup = useConfigurationGroup;
			item.ConfigurationId = configurationId;
            item.IsolationType = isolationType;
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			this._documentSession.SaveEvict(project);
			return item;
		}

		public void DeleteComponent(string projectId, string componentId)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			DeployProject project;
			if(!string.IsNullOrEmpty(projectId))
			{
				project = _documentSession.LoadEnsure<DeployProject>(projectId);
			}
			else 
			{
				project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.Id == componentId));
				if (project == null)
				{
					throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
				}
			}
			_logger.Info("User {0} deleting component {1}", _userIdentity.UserName, componentId);
			var component = project.ComponentList.FirstOrDefault(i => i.Id == componentId);
			if(component == null)
			{
				throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
			}
			project.ComponentList.Remove(component);
			this._documentSession.SaveEvict(project);
		}

		public List<DeployStep> GetComponentDeploymentStepList(string componentId)
		{
			var component = GetComponent(componentId);
			return component.DeploymentStepList;
		}
		public List<DeployStep> GetConfigurationDeploymentStepList(string configurationId)
		{
			var configuration = GetConfiguration(configurationId);
			return configuration.DeploymentStepList;
		}

		public DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId) 
		{
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Mising component ID");
            }
            if(string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if(string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task type name");
            }
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var component = project.ComponentList.SingleOrDefault(i => i.Id == componentId);
            if(component == null)
            {
                throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
            }
			var item = new DeployStep
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = project.Id,
				ParentId = componentId,
				ParentType = EnumDeployStepParentType.Component,
				StepName = stepName,
				TaskTypeName = taskTypeName,
				TaskOptionsJson = taskOptionsJson,
				SharedDeploymentStepId = StringHelper.IsNullOrEmpty(sharedDeploymentStepId, Guid.NewGuid().ToString()),
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			
			if(component.DeploymentStepList == null)
			{
				component.DeploymentStepList = new List<DeployStep>();
			}
			component.DeploymentStepList.Add(item);
			this._documentSession.SaveEvict(project);
			return item;
		}

		public DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson) 
		{
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
            if(string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if(string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task type name");
            }
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var configuration = project.ConfigurationList.SingleOrDefault(i => i.Id == configurationId);
            if(configuration == null)
            {
                throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
            }
			var item = new DeployStep
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = project.Id,
				ParentId = configurationId,
				ParentType = EnumDeployStepParentType.Configuration,
				StepName = stepName,
				TaskTypeName = taskTypeName,
				TaskOptionsJson = taskOptionsJson,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			
			if(configuration.DeploymentStepList == null)
			{
				configuration.DeploymentStepList = new List<DeployStep>();
			}
			configuration.DeploymentStepList.Add(item);
			this._documentSession.SaveEvict(project);
			return item;
		}

		public DeployStep GetComponentDeploymentStep(string deploymentStepId)
		{
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
			var project = this._documentSession.QueryNoCache<DeployProject>().SingleOrDefault(i=>i.ComponentList.Any(j=>j.DeploymentStepList.Any(k=>k.Id == deploymentStepId)));
			if(project == null)
			{
				throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
			}
			var item = project.ComponentList.Single(i=>i.DeploymentStepList.Any(j=>j.Id == deploymentStepId)).DeploymentStepList.First(i=>i.Id == deploymentStepId);
			return item;
		}
		
		public DeployStep GetConfigurationDeploymentStep(string deploymentStepId)
		{
            if (string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            var project = this._documentSession.QueryNoCache<DeployProject>().SingleOrDefault(i => i.ConfigurationList.Any(j => j.DeploymentStepList.Any(k => k.Id == deploymentStepId)));
			if(project == null)
			{
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
			var item = project.ConfigurationList.Single(i=>i.DeploymentStepList.Any(j=>j.Id == deploymentStepId)).DeploymentStepList.First(i=>i.Id == deploymentStepId);
			return item;
		}

		public DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId) 
		{
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
            if(string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if(string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task type name");
            }
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var component = project.ComponentList.SingleOrDefault(i => i.Id == componentId);
            if(component == null)
            {
                throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
            }
			DeployStep item;
			if(!string.IsNullOrEmpty(deploymentStepId))
			{
				item = component.DeploymentStepList.SingleOrDefault(i=>i.Id == deploymentStepId);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
                }
			}
			else if (!string.IsNullOrEmpty(sharedDeploymentStepId))
			{
				item = component.DeploymentStepList.SingleOrDefault(i=>i.SharedDeploymentStepId == sharedDeploymentStepId);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployStep), "SharedDeploymentStepId", sharedDeploymentStepId);
                }
			}
			else
			{
				throw new ArgumentNullException("Either deploymentStepId or sharedDeploymentStepId must be specified");
			}
			item.StepName = stepName;
			item.TaskTypeName = taskTypeName;
			item.TaskOptionsJson = taskOptionsJson;
			item.SharedDeploymentStepId = StringHelper.IsNullOrEmpty(sharedDeploymentStepId, item.SharedDeploymentStepId);
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			this._documentSession.SaveEvict(project);
			return item;
		}

		public DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson) 
		{
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
            if(string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if(string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task type name");
            }
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var component = project.ConfigurationList.SingleOrDefault(i => i.Id == configurationId);
            if(component == null)
            {
                throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
            }
			if(string.IsNullOrEmpty(deploymentStepId))
			{
				throw new ArgumentNullException("Missing deploymentStepId");
			}
			var item = component.DeploymentStepList.FirstOrDefault(i=>i.Id == deploymentStepId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
			}
			item.StepName = stepName;
			item.TaskTypeName = taskTypeName;
			item.TaskOptionsJson = taskOptionsJson;
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			this._documentSession.SaveEvict(project);
			return item;
			
		}
		
		public void DeleteComponentDeploymentStep(string deploymentStepId)
		{
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.DeploymentStepList.Any(k => k.Id == deploymentStepId)));
			if (project == null)
			{
				throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
			}
			_logger.Info("User {0} deleting component deployment step {1}", _userIdentity.UserName, deploymentStepId);
			var component = project.ComponentList.Single(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
			var item = component.DeploymentStepList.First(i => i.Id == deploymentStepId);
			component.DeploymentStepList.Remove(item);
			this._documentSession.SaveEvict(project);

		}

		public void DeleteConfigurationDeploymentStep(string deploymentStepId)
		{
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ConfigurationList.Any(j => j.DeploymentStepList.Any(k => k.Id == deploymentStepId)));
			if (project == null)
			{
				throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
			}
			_logger.Info("User {0} deleting configuration deployment step {1}", _userIdentity.UserName, deploymentStepId);
			var configuration = project.ConfigurationList.Single(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
			var item = configuration.DeploymentStepList.First(i => i.Id == deploymentStepId);
			configuration.DeploymentStepList.Remove(item);
			this._documentSession.SaveEvict(project);

		}
		
		public List<DeployProjectBranch> GetBranchList(string projectId)
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
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var branch = new DeployProjectBranch
			{
				Id = Guid.NewGuid().ToString(),
				BranchName = branchName,
				ProjectId = projectId,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			project.BranchList.Add(branch);
			this._documentSession.SaveEvict(project);
			return branch;
		}


		public DeployProjectBranch GetBranch(string branchId, string projectId=null)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var branch = this.TryGetBranch(branchId, projectId);
			if(branch == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
			}
			return branch;
		}

		public DeployProjectBranch TryGetBranch(string branchId, string projectId=null)
		{
			if (string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			DeployProject project;
			if(projectId != null)
			{
				project = TryGetProject(projectId);
                if(project == null)
                {
                    return null;
                }
			}
			else 
			{
				project = this._documentSession.QueryNoCache<DeployProject>().FirstOrDefault(i => i.BranchList.Any(j => j.Id == branchId));
			}
			if (project != null)
			{
				return project.BranchList.FirstOrDefault(i => i.Id == branchId);
			}
			else 
			{
				return null;
			}
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
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(branchName))
            {
                throw new ArgumentNullException("Missing branch name");
            }
			var project = this.GetProject(projectId);
			return project.BranchList.FirstOrDefault(i => i.BranchName == branchName);
		}

		public DeployProjectBranch GetOrCreateBranch(string projectId, string branchIdOrName)
		{
			int retryCounter = 5;
			while(true)
			{
				try 
				{
					string itemId;
					using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel=IsolationLevel.Serializable }))
					{
						var project = GetProject(projectId);
                        var item = project.BranchList.FirstOrDefault(i => i.Id == branchIdOrName);
						if(item != null)
						{
							itemId = item.Id;
						}
						else 
						{
                            item = project.BranchList.FirstOrDefault(i => i.BranchName == branchIdOrName);
							if(item != null)
							{
								itemId = item.Id;
							}
							else 
							{
                                item = CreateBranch(projectId, branchIdOrName);
								itemId = item.Id;
								transaction.Complete();
							}
						}
					}
					return GetBranch(itemId, projectId);
				}
				catch(Raven.Abstractions.Exceptions.ConcurrencyException exception)
				{
					retryCounter--;
					if(retryCounter <= 0)
					{
						throw;
					}
					else 
					{
						_logger.WarnException(string.Format("GetOrCreateBranch concurrency exception, {0} retries remaining: {1}", retryCounter, exception.ToString()), exception);
					}
				}
			}
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
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var branch = project.BranchList.FirstOrDefault(i=>i.Id == branchId);
			if(branch == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
			}
			branch.UpdatedByUserName = _userIdentity.UserName;
			branch.UpdatedDateTimeUtc = DateTime.UtcNow;
			branch.BranchName = branchName;
			this._documentSession.SaveEvict(project);
			return branch;
		}

		public void DeleteBranch(string branchId, string projectId)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			_logger.Info("User {0} deleting branch {1}", _userIdentity.UserName, branchId);
			var branch = project.BranchList.FirstOrDefault(i=>i.Id == branchId);
			if(branch == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
			}
			project.BranchList.Remove(branch);
			this._documentSession.SaveEvict(project);
		}


		public List<DeployEnvironment> GetEnvironmentList(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			var project = GetProject(projectId);
			return project.EnvironmentList;
		}

		public DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			if(string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException("Missing environment name");
			}
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var environment = new DeployEnvironment
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				EnvironmentName = environmentName,
				ComponentList = componentList.ToList(),
				ConfigurationList = configurationList.ToList(),
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
			};
			UpdateComponentList(environment.ComponentList, project, environment, EnumDeployStepParentType.Component);
			UpdateComponentList(environment.ConfigurationList, project, environment, EnumDeployStepParentType.Configuration);
			if(project.EnvironmentList == null)
			{
				project.EnvironmentList = new List<DeployEnvironment>();
			}
			project.EnvironmentList.Add(environment);
			this._documentSession.SaveEvict(project);
			return environment;
		}

		private void UpdateComponentList(IEnumerable<DeployEnvironmentConfiguration> componentList, DeployProject project, DeployEnvironment environment, EnumDeployStepParentType parentType)
		{
			foreach (var component in componentList)
			{
				if (string.IsNullOrEmpty(component.Id))
				{
					component.Id = Guid.NewGuid().ToString();
				    component.CreatedDateTimeUtc = DateTime.UtcNow;
				    component.CreatedByUserName = _userIdentity.UserName;
				}
				component.EnvironmentId = environment.Id;
				component.ProjectId = project.Id;
                component.ParentType = parentType;
				component.UpdatedDateTimeUtc = DateTime.UtcNow;
				component.UpdatedByUserName = _userIdentity.UserName;

				if (component.MachineList != null)
				{
					foreach (var machine in component.MachineList)
					{
						if (string.IsNullOrEmpty(machine.Id))
						{
							machine.Id = Guid.NewGuid().ToString();
                            machine.CreatedDateTimeUtc = DateTime.UtcNow;
                            machine.CreatedByUserName = _userIdentity.UserName;
                        }
						machine.ProjectId = project.Id;
						machine.ProjectName = project.ProjectName;
						machine.ParentId = component.Id;
						machine.EnvironmentId = environment.Id;
						machine.EnvironmentName = environment.EnvironmentName;
                        machine.UpdatedDateTimeUtc = DateTime.UtcNow;
                        machine.UpdatedByUserName = _userIdentity.UserName;
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
			var project = this._documentSession.QueryNoCache<DeployProject>()
							.ToList()
							.FirstOrDefault(i=>i.EnvironmentList.Any(j=>j.Id == environmentId));
			if(project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i=>i.Id == environmentId);
			return environment;
		}

		public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
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
			environment.ConfigurationList = configurationList.ToList();
			environment.UpdatedByUserName = _userIdentity.UserName;
			environment.UpdatedDateTimeUtc = DateTime.UtcNow;
			UpdateComponentList(componentList, project, environment, EnumDeployStepParentType.Component);
			UpdateComponentList(configurationList, project, environment, EnumDeployStepParentType.Configuration);
			this._documentSession.SaveEvict(project);
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
			_logger.Info("User {0} deleting environment {1}", _userIdentity.UserName, environmentId);
			var environment = project.EnvironmentList.First(i => i.Id == environmentId);
			project.EnvironmentList.Remove(environment);
			this._documentSession.SaveEvict(project);
		}


		public DeployMachine GetMachine(string machineId)
		{
			if (string.IsNullOrEmpty(machineId))
			{
				throw new ArgumentNullException("Missing machine ID");
			}
			var project = this._documentSession.QueryNoCache<DeployProject>().FirstOrDefault(i => i.EnvironmentList.Any(j => j.ComponentList.Any(k=>k.MachineList.Any(l=>l.Id == machineId))));
			if (project == null)
			{
				throw new ArgumentException("Unable to find project for machine ID " + machineId);
			}
			return project.GetMachine(machineId);
		}

		public DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configValueList)
		{
			var project = _documentSession.LoadEnsure<DeployProject>(projectId);
			var item = project.GetMachine(machineId);
			item.MachineName = machineId;
			item.ConfigurationValueList = configValueList;
			this._documentSession.SaveEvict(project);
			return item;
		}
	}
}
