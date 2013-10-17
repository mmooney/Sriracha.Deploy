using MMDB.Permissions;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class ProjectRoleManager : IProjectRoleManager
	{
		private readonly IPermissionManager _permissionManager;

		public ProjectRoleManager(IPermissionManager permissionManager)
		{
			_permissionManager = DIHelper.VerifyParameter(permissionManager);
		}

		private DeployProjectRole CreateDeployProjectRole(PermissionRole role)
		{
			return new DeployProjectRole
			{
				Id = role.Id,
				RoleName = role.RoleName,
				ProjectId = role.GetAssignmentValue("ProjectId")
			};
		}

		public DeployProjectRole GetProjectRole(string projectRoleId)
		{
			var role = _permissionManager.GetRole(projectRoleId);
			return CreateDeployProjectRole(role);
		}

		public List<DeployProjectRole> GetProjectRoleList(string projectId)
		{
			var assignment = new PermissionDataAssignment
			{
				DataObjectName = "ProjectId",
				DataObjectId = projectId
			};
			var roleList = _permissionManager.GetRoleList(assignment);
			return roleList.Select(i=>CreateDeployProjectRole(i)).ToList();
		}


		public DeployProjectRole CreateRole(string projectId, string roleName)
		{
			var assignment = new PermissionDataAssignment
			{
				DataObjectName = "ProjectId",
				DataObjectId = projectId
			};
			var role = _permissionManager.CreateRole(roleName, assignment.ListMe());
			return CreateDeployProjectRole(role);
		}

		public DeployProjectRole UpdateRole(string roleId, string projectId, string roleName)
		{
			var assignment = new PermissionDataAssignment
			{
				DataObjectName = "ProjectId",
				DataObjectId = projectId
			};
			var role = _permissionManager.UpdateRole(roleId, roleName, assignment.ListMe());
			return CreateDeployProjectRole(role);
		}
	}
}
