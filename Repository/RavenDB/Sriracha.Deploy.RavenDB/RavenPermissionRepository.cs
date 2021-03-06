﻿using MMDB.Shared;
using Raven.Client;
using Raven.Client.Linq;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenPermissionRepository : IPermissionRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;

		public RavenPermissionRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		private DeployProjectRoleAssignments UpdateAssignments(DeployProjectRoleAssignments assignments, DeployProjectRole role)
		{
			assignments = assignments??new DeployProjectRoleAssignments();
            //if(string.IsNullOrEmpty(assignments.Id))
            //{
            //    assignments.Id = Guid.NewGuid().ToString();
            //    assignments.CreatedByUserName = _userIdentity.UserName;
            //    assignments.CreatedDateTimeUtc = DateTime.UtcNow;
            //}
            //assignments.ProjectId = role.ProjectId;
            //assignments.ProjectRoleId = role.Id;
            //assignments.UpdatedByUserName = _userIdentity.UserName;
            //assignments.UpdateDateTimeUtc = DateTime.UtcNow;
			return assignments;
		}

		private DeployProjectRolePermissions UpdatePermissions(DeployProjectRolePermissions permissions, DeployProjectRole role)
		{
            permissions = permissions ?? new DeployProjectRolePermissions();
            //if(string.IsNullOrEmpty(permissions.Id))
            //{
            //    permissions.Id = Guid.NewGuid().ToString();
            //    permissions.CreatedByUserName = _userIdentity.UserName;
            //    permissions.CreatedDateTimeUtc = DateTime.UtcNow;
            //}
            //permissions.ProjectId = role.ProjectId;
            //permissions.ProjectRoleId = role.Id;
            //permissions.UpdatedByUserName = _userIdentity.UserName;
            //permissions.UpdateDateTimeUtc = DateTime.UtcNow;

			UpdateEnvironmentPermissions(role, permissions.RequestDeployPermissionList);
			UpdateEnvironmentPermissions(role, permissions.ApproveRejectDeployPermissionList);
			UpdateEnvironmentPermissions(role, permissions.RunDeploymentPermissionList);
			UpdateEnvironmentPermissions(role, permissions.EditEnvironmentPermissionList);
			UpdateEnvironmentPermissions(role, permissions.EditEnvironmentPermissionsPermissionList);
			return permissions;
		}

		private void UpdateEnvironmentPermissions(DeployProjectRole role, List<DeployProjectRoleEnvironmentPermission> list)
		{
            //if (list != null)
            //{
            //    foreach (var item in list)
            //    {
            //        if (string.IsNullOrEmpty(item.Id))
            //        {
            //            item.Id = Guid.NewGuid().ToString();
            //            item.CreatedByUserName = _userIdentity.UserName;
            //            item.CreatedDateTimeUtc = DateTime.UtcNow;
            //        }
            //        item.ProjectId = role.ProjectId;
            //        item.ProjectRoleId = role.Id;
            //        item.UpdatedByUserName = _userIdentity.UserName;
            //        item.UpdateDateTimeUtc = DateTime.UtcNow;
            //    }
            //}
		}

		public DeployProjectRole GetProjectRole(string projectRoleId)
		{
			if(string.IsNullOrEmpty(projectRoleId))
			{
				throw new ArgumentNullException("Missing projectRoleId");
			}
			var item = _documentSession.Load<DeployProjectRole>(projectRoleId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(DeployProjectRole), "Id", projectRoleId);
			}
			return item;
		}

		public List<DeployProjectRole> GetProjectRoleList(string projectId)
		{
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }
			return _documentSession.Query<DeployProjectRole>().Where(i=>i.ProjectId == projectId).ToList();
		}

		public List<DeployProjectRole> GetProjectRoleListForUser(string userName)
		{
            if(string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
			return _documentSession.Query<DeployProjectRole>().ToList().Where(i => i.Assignments.UserNameList.Any(j=>j == userName)).ToList();
		}

		public DeployProjectRole CreateProjectRole(string projectId, string projectName, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("projectId");
			}
            if(string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("projectName");
            }
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}
			var existingItem = _documentSession.QueryNoCacheNotStale<DeployProjectRole>().Any(i=>i.ProjectId == projectId && i.RoleName == roleName);
			if(existingItem)
			{
				throw new ArgumentException(string.Format("DeployProjectRole already exists for Project {0} and RoleName {1}", projectId, roleName));
			}
			var newItem = new DeployProjectRole
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ProjectName = projectName,
				RoleName = roleName,
				EveryoneRoleIndicator = everyoneRoleIndicator,
				CreatedByUserName = _userIdentity.UserName,
				CreatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow
			};
			newItem.Permissions = this.UpdatePermissions(permissions, newItem);
			newItem.Assignments = this.UpdateAssignments(assignments, newItem);
			_documentSession.Store(newItem);
			_documentSession.SaveChanges();
			return newItem;
		}

		public DeployProjectRole UpdateProjectRole(string roleId, string projectId, string projectName, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator)
		{
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }
            if(string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("projectName");
            }
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }
            var existingItem = _documentSession.QueryNoCacheNotStale<DeployProjectRole>().Any(i => i.Id != roleId && i.ProjectId == projectId && i.RoleName == roleName);
            if (existingItem)
            {
                throw new ArgumentException(string.Format("DeployProjectRole already exists for Project {0} and RoleName {1}", projectId, roleName));
            }
            var item = this.GetProjectRole(roleId);
            item.ProjectId = projectId;
			item.ProjectName = projectName;
			item.RoleName = roleName;
			item.Permissions = this.UpdatePermissions(permissions, item);
			item.Assignments = this.UpdateAssignments(assignments, item);
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			item.EveryoneRoleIndicator = everyoneRoleIndicator;
			_documentSession.SaveChanges();
			return item;
		}


		public DeployProjectRole TryGetProjectEveryoneRole(string projectId)
		{
			return _documentSession.Query<DeployProjectRole>().FirstOrDefault(i=>i.ProjectId == projectId && i.EveryoneRoleIndicator);
		}


		public DeployProjectRole DeleteProjectRole(string roleId)
		{
			var role = GetProjectRole(roleId);
			_documentSession.Delete(role);
			_documentSession.SaveChanges();
			return role;
		}
	}
}
