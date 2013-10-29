using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Project
{
	public interface IProjectRoleManager
	{
		DeployProjectRole GetProjectRole(string projectRoleId);
		List<DeployProjectRole> GetProjectRoleList(string projectId);
		List<DeployProjectRole> GetProjectRoleListForUser(string userName);

		DeployProjectRole CreateRole(string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator);
		DeployProjectRole UpdateRole(string roleId, string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator);
		DeployProjectRole DeleteRole(string roleId);

		DeployProjectRole GetProjectEveryoneRole(string projectId);
		DeployProjectRole TryGetProjectEveryoneRole(string projectId);

	}
}
