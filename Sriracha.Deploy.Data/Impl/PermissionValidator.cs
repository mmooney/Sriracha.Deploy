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

		public PermissionValidator(IProjectRoleManager projectRoleManager)
		{
			_projectRoleManager = DIHelper.VerifyParameter(projectRoleManager);
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
					EditComponentConfigurationAccess = projectRoleList.Any(i=>i.Permissions.EditComponentConfigurationAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
														: projectRoleList.Any(i=>i.Permissions.EditComponentConfigurationAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
														: EnumPermissionAccess.None,
					ApproveRejectDeployPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.ApproveRejectDeployPermissionList)),
					RequestDeployPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.RequestDeployPermissionList)),
					RunDeploymentPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.RunDeploymentPermissionList)),
					EditEnvironmentPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.EditEnvironmentPermissionList)),
					ManagePermissionsPermissionList = MergePermissions(projectRoleList.SelectMany(i => i.Permissions.ManagePermissionsPermissionList))
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
