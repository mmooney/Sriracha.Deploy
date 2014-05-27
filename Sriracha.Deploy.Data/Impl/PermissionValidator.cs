using MMDB.Shared;
using Sriracha.Deploy.Data.Account;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Exceptions;
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
        private readonly ISystemRoleManager _systemRoleManager;
		private readonly IUserIdentity _userIdentity;

		public PermissionValidator(IProjectRoleManager projectRoleManager, ISystemRoleManager systemRoleManager, IUserIdentity userIdentity)
		{
			_projectRoleManager = DIHelper.VerifyParameter(projectRoleManager);
            _systemRoleManager = DIHelper.VerifyParameter(systemRoleManager);
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
				var effectiveProjectPermissions = new DeployProjectEffectivePermissions
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

            var userSystemRoleList = _systemRoleManager.GetSystemRoleListForUser(userName);
            returnValue.SystemPermissions = new SystemRolePermissions
            {
                EditSystemPermissionsAccess = userSystemRoleList.Any(i => i.Permissions.EditSystemPermissionsAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
                                                : userSystemRoleList.Any(i => i.Permissions.EditSystemPermissionsAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
                                                : EnumPermissionAccess.None,
                EditUsersAccess = userSystemRoleList.Any(i => i.Permissions.EditUsersAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
                                                : userSystemRoleList.Any(i => i.Permissions.EditUsersAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
                                                : EnumPermissionAccess.None,
                EditDeploymentCredentialsAccess = userSystemRoleList.Any(i => i.Permissions.EditDeploymentCredentialsAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
                                                : userSystemRoleList.Any(i => i.Permissions.EditDeploymentCredentialsAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
                                                : EnumPermissionAccess.None,
                EditBuildPurgeRulesAccess = userSystemRoleList.Any(i => i.Permissions.EditBuildPurgeRulesAccess == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
                                                : userSystemRoleList.Any(i => i.Permissions.EditBuildPurgeRulesAccess == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
                                                : EnumPermissionAccess.None,

            };


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
                    //ProjectId = environmentPermissionList.Select(i=>i.ProjectId).FirstOrDefault(),
					Access = environmentPermissionList.Any(i=>i.Access == EnumPermissionAccess.Deny) ? EnumPermissionAccess.Deny
								: environmentPermissionList.Any(i=>i.Access == EnumPermissionAccess.Grant) ? EnumPermissionAccess.Grant
								: EnumPermissionAccess.None
				};
				returnList.Add(item);
			}
			return returnList;
		}

        public void VerifyCurrentUserSystemPermission(EnumSystemPermission permission)
        {
            this.VerifySystemPermission(_userIdentity.UserName, permission);
        }

        public bool CurrentUserHasSystemPermission(EnumSystemPermission permission)
        {
            return this.HasSystemPermission(_userIdentity.UserName, permission);
        }

        public void VerifySystemPermission(string userName, EnumSystemPermission permission)
        {
            if(!this.HasSystemPermission(userName, permission))
            {
                throw new SystemPermissionDeniedException(userName, permission);
            }
        }

        public bool HasSystemPermission(string userName, EnumSystemPermission permission)
        {
            var permissionData = this.GetUserEffectivePermissions(this._userIdentity.UserName);
            if(permissionData == null || permissionData.SystemPermissions == null)
            {
                return false;
            }
            EnumPermissionAccess access;
            switch(permission)
            {
                case EnumSystemPermission.EditSystemPermissions:
                    access = permissionData.SystemPermissions.EditSystemPermissionsAccess;
                    break;
                case EnumSystemPermission.EditUsers:
                    access = permissionData.SystemPermissions.EditUsersAccess;
                    break;
                case EnumSystemPermission.EditDeploymentCredentials:
                    access = permissionData.SystemPermissions.EditDeploymentCredentialsAccess;
                    break;
                case EnumSystemPermission.EditBuildPurgeRules:
                    access = permissionData.SystemPermissions.EditBuildPurgeRulesAccess;
                    break;
                default:
                    throw new UnknownEnumValueException(permission);
            }
            return (access == EnumPermissionAccess.Grant);
        }
    }
}
