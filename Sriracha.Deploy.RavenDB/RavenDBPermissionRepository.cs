using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenDBPermissionRepository : IPermissionRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;

		public RavenDBPermissionRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		private DeployProjectRoleAssignments UpdateAssignments(DeployProjectRoleAssignments assignments, DeployProjectRole role)
		{
			assignments = assignments??new DeployProjectRoleAssignments();
			if(string.IsNullOrEmpty(assignments.Id))
			{
				assignments.Id = Guid.NewGuid().ToString();
				assignments.CreatedByUserName = _userIdentity.UserName;
				assignments.CreatedDateTimeUtc = DateTime.UtcNow;
			}
			assignments.ProjectId = role.ProjectId;
			assignments.ProjectRoleId = role.Id;
			assignments.UpdatedByUserName = _userIdentity.UserName;
			assignments.UpdateDateTimeUtc = DateTime.UtcNow;
			return assignments;
		}

		private DeployProjectRolePermissions UpdatePermissions(DeployProjectRolePermissions permissions, DeployProjectRole role)
		{
			if(permissions == null)
			{
				permissions = new DeployProjectRolePermissions();
			}
			if(string.IsNullOrEmpty(permissions.Id))
			{
				permissions.Id = Guid.NewGuid().ToString();
				permissions.CreatedByUserName = _userIdentity.UserName;
				permissions.CreatedDateTimeUtc = DateTime.UtcNow;
			}
			permissions.ProjectId = role.ProjectId;
			permissions.ProjectRoleId = role.Id;
			permissions.UpdatedByUserName = _userIdentity.UserName;
			permissions.UpdateDateTimeUtc = DateTime.UtcNow;

			UpdateEnvironmentPermissions(role, permissions.RequestDeployPermissionList);
			UpdateEnvironmentPermissions(role, permissions.ApproveRejectDeployPermissionList);
			UpdateEnvironmentPermissions(role, permissions.RunDeploymentPermissionList);
			UpdateEnvironmentPermissions(role, permissions.EditEnvironmentPermissionList);
			UpdateEnvironmentPermissions(role, permissions.ManagePermissionsPermissionList);
			return permissions;
		}

		private void UpdateEnvironmentPermissions(DeployProjectRole role, List<DeployProjectRoleEnvironmentPermission> list)
		{
			if (list != null)
			{
				foreach (var item in list)
				{
					if (string.IsNullOrEmpty(item.Id))
					{
						item.Id = Guid.NewGuid().ToString();
						item.CreatedByUserName = _userIdentity.UserName;
						item.CreatedDateTimeUtc = DateTime.UtcNow;
					}
					item.ProjectId = role.ProjectId;
					item.ProjectRoleId = role.Id;
					item.UpdatedByUserName = _userIdentity.UserName;
					item.UpdateDateTimeUtc = DateTime.UtcNow;
				}
			}
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
			return _documentSession.Query<DeployProjectRole>().Where(i=>i.ProjectId == projectId).ToList();
		}

		public DeployProjectRole CreateProjectRole(string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing projectId");
			}
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("Missing roleName");
			}
			var existingItem = _documentSession.Query<DeployProjectRole>().Any(i=>i.ProjectId == projectId && i.RoleName == roleName);
			if(existingItem)
			{
				throw new ArgumentNullException(string.Format("DeployProjectRole already exists for Project {0} and RoleName {1}", projectId, roleName));
			}
			var newItem = new DeployProjectRole
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				RoleName = roleName,
				CreatedByUserName = _userIdentity.UserName,
				CreatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
				UpdateDateTimeUtc = DateTime.UtcNow
			};
			newItem.Permissions = this.UpdatePermissions(permissions, newItem);
			newItem.Assignments = this.UpdateAssignments(assignments, newItem);
			_documentSession.Store(newItem);
			_documentSession.SaveChanges();
			return newItem;
		}

		public DeployProjectRole UpdateProjectRole(string roleId, string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments)
		{
			var item = this.GetProjectRole(roleId);
			item.RoleName = roleName;
			item.Permissions = this.UpdatePermissions(permissions, item);
			item.Assignments = this.UpdateAssignments(assignments, item);
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdateDateTimeUtc = DateTime.UtcNow;
			_documentSession.SaveChanges();
			return item;
		}
	}
}
