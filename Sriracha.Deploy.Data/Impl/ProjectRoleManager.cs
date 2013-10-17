using MMDB.Permissions;
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
		private readonly IPermissionManager _permissionManager;
		private readonly IProjectRepository _projectRepository;

		public ProjectRoleManager(IPermissionManager permissionManager, IProjectRepository projectRepository)
		{
			_permissionManager = DIHelper.VerifyParameter(permissionManager);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
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

		private DeployProjectRole LoadRolePermissions(DeployProjectRole role, DeployProject project)
		{
			role.Permissions = role.Permissions ?? new DeployProjectRolePermissions();

			var allPermissions = _permissionManager.GetRolePermissionList(role.Id);

			role.Permissions.RequestDeployPermissionList =  (from i in allPermissions
															select this.CreateEnvironentPermission(i)).ToList();
			this.UpdateEnvironmentPermissions(role.Permissions.RequestDeployPermissionList, role, project);

			return role;
		}

		private void UpdateEnvironmentPermissions(List<DeployProjectRoleEnvironmentPermission> list, DeployProjectRole projectRole, DeployProject project)
		{
			var itemsToRemove = new List<DeployProjectRoleEnvironmentPermission>();
			var itemsToAdd = new List<DeployProjectRoleEnvironmentPermission>();
			foreach (var item in list)
			{
				var environment = project.EnvironmentList.FirstOrDefault(i=>i.Id == item.EnvironmentId);
				if(environment == null)
				{
					itemsToRemove.Add(item);
				}
				else 
				{
					item.EnvironmentName = environment.EnvironmentName;
				}
			}
			foreach (var item in itemsToRemove)
			{
				list.Remove(item);
			}
			foreach (var environment in project.EnvironmentList)
			{
				var item = list.FirstOrDefault(i=>i.EnvironmentId == environment.Id);
				if(item == null)
				{
					item = new DeployProjectRoleEnvironmentPermission
					{
						EnvironmentId = environment.Id,
						EnvironmentName = environment.EnvironmentName,
						Access = EnumPermissionAccess.None,
						ProjectId = project.Id,
						ProjectRoleId = projectRole.Id
					};
					list.Add(item);
				}
			}
		}

		private DeployProjectRoleEnvironmentPermission CreateEnvironentPermission(RolePermission rolePermission)
		{
			return new DeployProjectRoleEnvironmentPermission
			{
				Id = rolePermission.Id,
				ProjectRoleId = rolePermission.RoleId,
				EnvironmentId = rolePermission.GetAssignmentValue("EnvironmentId"),
				ProjectId = rolePermission.GetAssignmentValue("ProjectId"),
				Access = rolePermission.Access
			};
		}

		public DeployProjectRole GetProjectRole(string projectRoleId)
		{
			var role = _permissionManager.GetRole(projectRoleId);
			var returnValue = this.CreateDeployProjectRole(role);
			string projectId = role.GetAssignmentValue("ProjectId");
			var project = _projectRepository.GetProject(projectId);
			return this.LoadRolePermissions(returnValue, project);
		}

		public List<DeployProjectRole> GetProjectRoleList(string projectId)
		{
			var assignment = new PermissionDataAssignment
			{
				DataPropertyName = "ProjectId",
				DataPropertyValue = projectId
			};
			var roleList = _permissionManager.GetRoleList(assignment);
			return roleList.Select(i=>CreateDeployProjectRole(i)).ToList();
		}


		public DeployProjectRole CreateRole(string projectId, string roleName)
		{
			var assignment = new PermissionDataAssignment
			{
				DataPropertyName = "ProjectId",
				DataPropertyValue = projectId
			};
			var role = _permissionManager.CreateRole(roleName, assignment.ListMe());
			return CreateDeployProjectRole(role);
		}

		public DeployProjectRole UpdateRole(string roleId, string projectId, string roleName)
		{
			var assignment = new PermissionDataAssignment
			{
				DataPropertyName = "ProjectId",
				DataPropertyValue = projectId
			};
			var role = _permissionManager.UpdateRole(roleId, roleName, assignment.ListMe());
			return CreateDeployProjectRole(role);
		}
	}
}
