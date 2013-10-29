using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Project.ProjectImpl;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Project.ProjectImpl
{
	public class ProjectRoleManager : IProjectRoleManager
	{
		private readonly IPermissionRepository _permissionRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IMembershipRepository _membershipRepository;

		public ProjectRoleManager(IPermissionRepository permissionRepository, IProjectRepository projectRepository, IMembershipRepository membershipRepository)
		{
			_permissionRepository = DIHelper.VerifyParameter(permissionRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_membershipRepository = DIHelper.VerifyParameter(membershipRepository);
		}

		private DeployProjectRole ValidateRole(DeployProjectRole role, DeployProject project)
		{
			role.Permissions = this.ValidatePermissions(role.Permissions, role.Id, project);
			role.ProjectName = project.ProjectName;
			return role;
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

		private DeployProjectRoleAssignments ValidateAssignments(DeployProjectRoleAssignments assignments)
		{
			assignments = assignments ?? new DeployProjectRoleAssignments();

			var userNamesToRemove = new List<string>();
			if(assignments.UserNameList != null)
			{
				foreach(var userName in assignments.UserNameList)
				{
					if(!_membershipRepository.UserNameExists(userName))
					{
						assignments.UserNameList.Remove(userName);
					}
				}	
			}
			foreach(var userName in userNamesToRemove)
			{
				assignments.UserNameList.Remove(userName);
			}
			return assignments;
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
						Access = EnumPermissionAccess.Grant
					};
					permissionList.Add(item);
				}
			}
			
		}

		public DeployProjectRole GetProjectRole(string projectRoleId)
		{
			var role = _permissionRepository.GetProjectRole(projectRoleId);
			var project = _projectRepository.GetProject(role.ProjectId);
			this.ValidateRole(role, project);
			return role;
		}


		public List<DeployProjectRole> GetProjectRoleListForUser(string userName)
		{
			var projectList = _projectRepository.GetProjectList();
			var roleList = _permissionRepository.GetProjectRoleListForUser(userName);
			foreach(var role in roleList)
			{
				DeployProject project = projectList.Single(i=>i.Id == role.ProjectId);
				this.ValidateRole(role, project);
			}
			foreach(var project in projectList)
			{
				var everyoneRole = _permissionRepository.TryGetProjectEveryoneRole(project.Id);
				if(everyoneRole == null)
				{
					everyoneRole = CreateEveryoneRole(project);
				}
				this.ValidateRole(everyoneRole, project);
				roleList.Add(everyoneRole);
			}
			return roleList;
		}

		public List<DeployProjectRole> GetProjectRoleList(string projectId)
		{
			var project = _projectRepository.GetProject(projectId);
			var roleList = _permissionRepository.GetProjectRoleList(projectId);
			if(!roleList.Any(i=>i.EveryoneRoleIndicator))
			{
				var everyoneRole = CreateEveryoneRole(project);
				roleList.Add(everyoneRole);
			}
			foreach(var role in roleList)
			{
				this.ValidateRole(role, project);
			}
			return roleList;
		}

		private DeployProjectRole CreateEveryoneRole(DeployProject project)
		{
			var role = new DeployProjectRole
			{
				Id = "Everyone",
				ProjectId = project.Id,
				ProjectName = project.ProjectName,
				RoleName = "Everyone",
				EveryoneRoleIndicator = true
			};
			role.Permissions = this.ValidatePermissions(role.Permissions, role.Id, project);
			role.Permissions.EditComponentConfigurationAccess = EnumPermissionAccess.Grant;
			role.Permissions.CreateEnvironmentAccess = EnumPermissionAccess.Grant;
			foreach (var item in role.Permissions.RequestDeployPermissionList)
			{
				item.Access = EnumPermissionAccess.Grant;
			}
			foreach(var item in role.Permissions.RunDeploymentPermissionList)
			{
				item.Access = EnumPermissionAccess.Grant;
			}
			foreach(var item in role.Permissions.ApproveRejectDeployPermissionList)
			{
				item.Access = EnumPermissionAccess.Grant;
			}
			foreach(var item in role.Permissions.EditEnvironmentPermissionList)
			{
				item.Access = EnumPermissionAccess.Grant;
			}
			foreach(var item in role.Permissions.ManagePermissionsPermissionList)
			{
				item.Access = EnumPermissionAccess.Grant;
			}
			return role;
		}

		public DeployProjectRole GetProjectEveryoneRole(string projectId)
		{
			var project = _projectRepository.GetProject(projectId);
			var role = _permissionRepository.TryGetProjectEveryoneRole(projectId);
			if(role == null)
			{
				role = CreateEveryoneRole(project);
			}
			role.Permissions = this.ValidatePermissions(role.Permissions, "Everyone", project);
			return role;
		}

		public DeployProjectRole TryGetProjectEveryoneRole(string projectId)
		{
			return _permissionRepository.TryGetProjectEveryoneRole(projectId);
		}

		public DeployProjectRole CreateRole(string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator)
		{
			var project = _projectRepository.GetProject(projectId);
			permissions = this.ValidatePermissions(permissions, null, project);
			assignments = this.ValidateAssignments(assignments);
			return _permissionRepository.CreateProjectRole(projectId, project.ProjectName, roleName, permissions, assignments, everyoneRoleIndicator);
		}

		public DeployProjectRole UpdateRole(string roleId, string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator)
		{
			var project = _projectRepository.GetProject(projectId);
			permissions = this.ValidatePermissions(permissions, roleId, project);
			assignments = this.ValidateAssignments(assignments);
			return _permissionRepository.UpdateProjectRole(roleId, projectId, project.ProjectName, roleName, permissions, assignments, everyoneRoleIndicator);
		}
	}
}
