using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class ProjectRoleManager : IProjectRoleManager
	{
		private readonly IPermissionRepository _permissionRepository;
		private readonly IProjectRepository _projectRepository;

		public ProjectRoleManager(IPermissionRepository permissionRepository, IProjectRepository projectRepository)
		{
			_permissionRepository = DIHelper.VerifyParameter(permissionRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
		}

		private DeployProjectRolePermissions ValidatePermissions(DeployProjectRolePermissions permissions, string projectRoleId, DeployProject project)
		{
			permissions = permissions ?? new DeployProjectRolePermissions();

			permissions.RequestDeployPermissionList = permissions.RequestDeployPermissionList ?? new List<DeployProjectRoleEnvironmentPermission>();
			this.ValidateEnvironmentPermissions(permissions.RequestDeployPermissionList, projectRoleId, project);

			permissions.ApproveRejectDeployPermissionList = permissions.ApproveRejectDeployPermissionList ?? new List<DeployProjectRoleEnvironmentPermission>();
			this.ValidateEnvironmentPermissions(permissions.ApproveRejectDeployPermissionList, projectRoleId, project);

			permissions.RunDeploymentPermissionList = permissions.RunDeploymentPermissionList ?? new List<DeployProjectRoleEnvironmentPermission>();
			this.ValidateEnvironmentPermissions(permissions.RunDeploymentPermissionList, projectRoleId, project);

			permissions.EditEnvironmentPermissionList = permissions.EditEnvironmentPermissionList ?? new List<DeployProjectRoleEnvironmentPermission>();
			this.ValidateEnvironmentPermissions(permissions.EditEnvironmentPermissionList, projectRoleId, project);

			permissions.ManagePermissionsPermissionList = permissions.ManagePermissionsPermissionList ?? new List<DeployProjectRoleEnvironmentPermission>();
			this.ValidateEnvironmentPermissions(permissions.ManagePermissionsPermissionList, projectRoleId, project);

			return permissions;
		}

		private void ValidateEnvironmentPermissions(List<DeployProjectRoleEnvironmentPermission> permissionList, string projectRoleId, DeployProject project)
		{
			var itemsToDelete = new List<DeployProjectRoleEnvironmentPermission>();
			foreach(var item in permissionList)
			{
				var environment = project.EnvironmentList.FirstOrDefault(i=>i.Id == item.EnvironmentId);
				if(environment == null)
				{
					itemsToDelete.Add(item);
				}
				else 
				{
					item.EnvironmentName = environment.EnvironmentName;
				}
			}
			foreach(var item in itemsToDelete)
			{
				permissionList.Remove(item);
			}
			foreach(var environment in project.EnvironmentList)
			{
				var item = permissionList.FirstOrDefault(i=>i.EnvironmentId == environment.Id);
				if(item == null)
				{
					item = new DeployProjectRoleEnvironmentPermission
					{
						EnvironmentId = environment.Id,
						EnvironmentName = environment.EnvironmentName,
						ProjectId = project.Id,
						ProjectRoleId = projectRoleId,
						Access = EnumPermissionAccess.None
					};
					permissionList.Add(item);
				}
			}
			
		}

		public DeployProjectRole GetProjectRole(string projectRoleId)
		{
			var role = _permissionRepository.GetProjectRole(projectRoleId);
			var project = _projectRepository.GetProject(role.ProjectId);
			role.Permissions = this.ValidatePermissions(role.Permissions, role.Id, project);
			return role;
		}


		public List<DeployProjectRole> GetProjectRoleList(string projectId)
		{
			var project = _projectRepository.GetProject(projectId);
			var roleList = _permissionRepository.GetProjectRoleList(projectId);
			foreach(var role in roleList)
			{
				role.Permissions = this.ValidatePermissions(role.Permissions, role.Id, project);
			}
			return roleList;
		}


		public DeployProjectRole CreateRole(string projectId, string roleName, DeployProjectRolePermissions permissions)
		{
			var project = _projectRepository.GetProject(projectId);
			permissions = this.ValidatePermissions(permissions, null, project);
			return _permissionRepository.CreateProjectRole(projectId, roleName, permissions);
		}

		public DeployProjectRole UpdateRole(string roleId, string projectId, string roleName, DeployProjectRolePermissions permissions)
		{
			var project = _projectRepository.GetProject(projectId);
			permissions = this.ValidatePermissions(permissions, roleId, project);
			return _permissionRepository.UpdateProjectRole(roleId, projectId, roleName, permissions);
		}
	}
}
