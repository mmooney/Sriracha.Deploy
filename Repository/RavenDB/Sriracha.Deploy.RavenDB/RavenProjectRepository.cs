﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MMDB.Shared;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Project;
using Common.Logging;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenProjectRepository : IProjectRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;
		private readonly ILog _logger;

		public RavenProjectRepository(IDocumentSession documentSession, IUserIdentity userIdentity, ILog logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public IEnumerable<DeployProject> GetProjectList()
		{
            return _documentSession.QueryNoCacheNotStale<DeployProject>().Customize(i => i.NoCaching()).Customize(i => i.NoTracking());
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
							item = _documentSession.QueryNoCacheNotStale<DeployProject>()
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
						_logger.Warn(string.Format("GetOrCreateProject concurrency exception, {0} retries remaining: {1}", retryCounter, exception.ToString()), exception);
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
            var list = _documentSession.QueryNoCacheNotStale<DeployProject>()
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
			_logger.Info(string.Format("User {0} deleting project {1}", _userIdentity.UserName, projectId));
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
			var project = _documentSession.LoadEnsureNoCache<DeployProject>(projectId);
			return project.ConfigurationList;
		}

		public DeployConfiguration GetConfiguration(string configurationId, string projectId)
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

        public void DeleteConfiguration(string configurationId, string projectId)
		{
			if (string.IsNullOrEmpty(configurationId))
			{
				throw new ArgumentNullException("Missing Configuration ID");
			}
			var project = this._documentSession.LoadEnsure<DeployProject>(projectId);
			_logger.Info(string.Format("User {0} deleting configuration {1}", _userIdentity.UserName, configurationId));
			var configuration = project.ConfigurationList.FirstOrDefault(i => i.Id == configurationId);
            if(configuration == null)
            {
                throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
            }
			project.ConfigurationList.Remove(configuration);
			_documentSession.SaveEvict(project);
		}

		public DeployConfiguration TryGetConfiguration(string configurationId, string projectId)
		{
            DeployProject project;
            if(!string.IsNullOrEmpty(projectId))
            {
                project = _documentSession.LoadNoCache<DeployProject>(projectId);
            }
            else 
            {
                project = _documentSession.QueryNoCacheNotStale<DeployProject>()
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
			var project = _documentSession.LoadEnsureNoCache<DeployProject>(projectId);
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

		public DeployComponent GetComponent(string componentId, string projectId)
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

		public DeployComponent TryGetComponent(string componentId, string projectId)
		{
			if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
			if(string.IsNullOrEmpty(projectId))
			{
                throw new ArgumentNullException("Missing projet ID");
			}
			var project = _documentSession.LoadNoCache<DeployProject>(projectId);
			if(project == null)
			{
				return null;
			}
			else 
			{
				return project.ComponentList.FirstOrDefault(i=>i.Id == componentId);
			}
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
						_logger.Warn(string.Format("GetOrCreateComponent concurrency exception, {0} retries remaining: {1}", retryCounter, exception.ToString()), exception);
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
                project = this._documentSession.QueryNotStale<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.Id == componentId));
				if (project == null)
				{
					throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
				}
			}
			_logger.Info(string.Format("User {0} deleting component {1}", _userIdentity.UserName, componentId));
			var component = project.ComponentList.FirstOrDefault(i => i.Id == componentId);
			if(component == null)
			{
				throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
			}
			project.ComponentList.Remove(component);
			this._documentSession.SaveEvict(project);
		}

		public List<DeployStep> GetComponentDeploymentStepList(string componentId, string projectId)
		{
			var component = GetComponent(componentId, projectId);
			return component.DeploymentStepList;
		}
        public List<DeployStep> GetConfigurationDeploymentStepList(string configurationId, string projectId)
		{
			var configuration = GetConfiguration(configurationId, projectId);
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
            item.OrderNumber = component.DeploymentStepList.Count();
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
            item.OrderNumber = configuration.DeploymentStepList.Count();
			configuration.DeploymentStepList.Add(item);
			this._documentSession.SaveEvict(project);
			return item;
		}

        public DeployStep GetComponentDeploymentStep(string deploymentStepId, string projectId)
		{
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            var project = this._documentSession.LoadEnsureNoCache<DeployProject>(projectId);
			return project.GetComponentDeploymentStep(deploymentStepId);
		}

        public DeployStep GetConfigurationDeploymentStep(string deploymentStepId, string projectId)
		{
            if (string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            var project = this._documentSession.LoadEnsureNoCache<DeployProject>(projectId);
            return project.GetConfigurationDeploymentStep(deploymentStepId);
		}

		public DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId, int? orderNumber=null) 
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
            if (orderNumber.HasValue)
            {
                item.OrderNumber = orderNumber.Value;
            }
            this._documentSession.SaveEvict(project);
			return item;
		}

		public DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson, int? orderNumber) 
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
            if(orderNumber.HasValue)
            {
                item.OrderNumber = orderNumber.Value;
            }
			this._documentSession.SaveEvict(project);
			return item;
			
		}

        public void DeleteComponentDeploymentStep(string deploymentStepId, string projectId)
		{
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            var project = this._documentSession.LoadEnsure<DeployProject>(projectId);
			_logger.Info(string.Format("User {0} deleting component deployment step {1}", _userIdentity.UserName, deploymentStepId));
			var component = project.ComponentList.SingleOrDefault(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
            if(component == null)
            {
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
			var item = component.DeploymentStepList.FirstOrDefault(i => i.Id == deploymentStepId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
			component.DeploymentStepList.Remove(item);
			this._documentSession.SaveEvict(project);

		}

        public void DeleteConfigurationDeploymentStep(string deploymentStepId, string projectId)
		{
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            var project = this._documentSession.LoadEnsure<DeployProject>(projectId);
			if (project == null)
			{
				throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
			}
			_logger.Info(string.Format("User {0} deleting configuration deployment step {1}", _userIdentity.UserName, deploymentStepId));
			var configuration = project.ConfigurationList.SingleOrDefault(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
            if(configuration == null)
            {
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
			var item = configuration.DeploymentStepList.FirstOrDefault(i => i.Id == deploymentStepId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
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


		public DeployProjectBranch GetBranch(string branchId, string projectId)
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

		public DeployProjectBranch TryGetBranch(string branchId, string projectId)
		{
			if (string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            var project = TryGetProject(projectId);
            if(project == null)
            {
                return null;
            }
			else 
			{
				return project.BranchList.FirstOrDefault(i => i.Id == branchId);
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
						_logger.Warn(string.Format("GetOrCreateBranch concurrency exception, {0} retries remaining: {1}", retryCounter, exception.ToString()), exception);
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
			_logger.Info(string.Format("User {0} deleting branch {1}", _userIdentity.UserName, branchId));
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
				ComponentList = (componentList ?? new List<DeployEnvironmentConfiguration>()).ToList(),
				ConfigurationList = (configurationList ?? new List<DeployEnvironmentConfiguration>()).ToList(),
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
                switch(parentType)
                {
                    case EnumDeployStepParentType.Component:
                        if(string.IsNullOrEmpty(component.ParentId))
                        {
                            throw new ArgumentNullException("Missing component parent ID");
                        }
                        if(!project.ComponentList.Any(i=>i.Id == component.ParentId))
                        {
                            throw new RecordNotFoundException(typeof(DeployComponent), "Id", component.ParentId);
                        }
                        break;
                    case EnumDeployStepParentType.Configuration:
                        if(string.IsNullOrEmpty(component.ParentId))
                        {
                            throw new ArgumentNullException("Missing configuration parent ID");
                        }
                        if(!project.ConfigurationList.Any(i=>i.Id == component.ParentId))
                        {
                            throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", component.ParentId);
                        }
                        break;
                    default:
                        throw new UnknownEnumValueException(parentType);
                }
				if (string.IsNullOrEmpty(component.Id))
				{
					component.Id = Guid.NewGuid().ToString();
				    component.CreatedDateTimeUtc = DateTime.UtcNow;
				    component.CreatedByUserName = _userIdentity.UserName;
				}
				component.EnvironmentId = environment.Id;
				component.ProjectId = environment.ProjectId;
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
						machine.ProjectId = environment.ProjectId;
						machine.ParentId = component.Id;
						machine.EnvironmentId = environment.Id;
						machine.EnvironmentName = environment.EnvironmentName;
                        machine.UpdatedDateTimeUtc = DateTime.UtcNow;
                        machine.UpdatedByUserName = _userIdentity.UserName;
                    }
				}
			}
		}

        public DeployEnvironment GetEnvironment(string environmentId, string projectId)
		{
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
            var project = this._documentSession.LoadEnsureNoCache<DeployProject>(projectId);
			var environment = project.EnvironmentList.SingleOrDefault(i=>i.Id == environmentId);
            if(environment == null)
            {
                throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environmentId);
            }
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
            var project = this._documentSession.LoadEnsure<DeployProject>(projectId);
			var environment = project.EnvironmentList.SingleOrDefault(i => i.Id == environmentId);
            if(environment == null)
            {
                throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environmentId);
            }
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

        public void DeleteEnvironment(string environmentId, string projectId)
		{
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
            var project = this._documentSession.LoadEnsure<DeployProject>(projectId);
			_logger.Info(string.Format("User {0} deleting environment {1}", _userIdentity.UserName, environmentId));
			var environment = project.EnvironmentList.SingleOrDefault(i => i.Id == environmentId);
            if(environment == null)
            {
                throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environment);
            }
			project.EnvironmentList.Remove(environment);
			this._documentSession.SaveEvict(project);
		}

	}
}
