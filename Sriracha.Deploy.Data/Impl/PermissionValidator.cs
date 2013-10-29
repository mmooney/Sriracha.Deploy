using MMDB.Shared;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class PermissionValidator : IPermissionValidator
	{
		private readonly IProjectRoleManager _projectRoleManager;
		private readonly IUserIdentity _userIdentity;

		public PermissionValidator(IProjectRoleManager projectRoleManager, IUserIdentity userIdentity)
		{
			_projectRoleManager = DIHelper.VerifyParameter(projectRoleManager);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public UserEffectivePermissions GetCurrentUserEffectivePermissions()
		{
			return this.GetUserEffectivePermissions(this._userIdentity.UserName);
		}

		public UserEffectivePermissions GetUserEffectivePermissions(string userName)
		{
			var returnValue = new UserEffectivePermissions
			{
				UserName = userName
			};
			
			var userRoleList = _projectRoleManager.GetProjectRoleListForUser(userName);
			var projectIdList = userRoleList.Select(i=>i.ProjectId).Distinct();
			foreach(var projectId in projectIdList)
			{
				var projectRoleList = userRoleList.Where(i=>i.ProjectId == projectId);
				var effectiveProjectPermissions = new DeployProjectRolePermissions
				{
					ProjectId = projectId,
					ProjectName = StringHelper.IsNullOrEmpty(projectRoleList.Select(i=>i.ProjectName).FirstOrDefault(), "(No Project Name)"),
					EditComponentConfigurationAccess = projectRoleList.Any(i => i.Permissions.EditComponentConfigurationAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
														: projectRoleList.Any(i => i.Permissions.EditComponentConfigurationAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
														: EnumPermissionAccess.None,
					CreateEnvironmentAccess = projectRoleList.Any(i => i.Permissions.CreateEnvironmentAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
														: projectRoleList.Any(i => i.Permissions.CreateEnvironmentAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
														: EnumPermissionAccess.None,
					EditProjectPermissionsAccess = projectRoleList.Any(i => i.Permissions.EditProjectPermissionsAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
														: projectRoleList.Any(i => i.Permissions.EditProjectPermissionsAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
														: EnumPermissionAccess.None,
					ApproveRejectDeployPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.ApproveRejectDeployPermissionList)),
					RequestDeployPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.RequestDeployPermissionList)),
					RunDeploymentPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.RunDeploymentPermissionList)),
					EditEnvironmentPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.EditEnvironmentPermissionList)),
					EditEnvironmentPermissionsPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.EditEnvironmentPermissionsPermissionList))
				};
				returnValue.ProjectPermissionList.Add(effectiveProjectPermissions);
			}

			return returnValue;
		}

		private List<DeployProjectRoleEnvironmentPermission> MergePermissions(IEnumerable<DeployProjectRoleEnvironmentPermission> permissionList)
		{
			var returnList = new List<DeployProjectRoleEnvironmentPermission>();
			var environmentIdList = permissionList.Select(i=>i.EnvironmentId).Distinct();
			foreach(var environmentId in environmentIdList)
			{
				var environmentPermissionList = permissionList.Where(i=>i.EnvironmentId == environmentId);
				var item = new DeployProjectRoleEnvironmentPermission
				{
					EnvironmentId = environmentId,
					EnvironmentName = StringHelper.IsNullOrEmpty(environmentPermissionList.Select(i=>i.EnvironmentName).FirstOrDefault(), environmentId),
					ProjectId = environmentPermissionList.Select(i=>i.ProjectId).FirstOrDefault(),
					Access = environmentPermissionList.Any(i=>i.Access == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
								: environmentPermissionList.Any(i=>i.Access == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
								: EnumPermissionAccess.None
				};
				returnList.Add(item);
			}
			return returnList;
		}


	}
}
