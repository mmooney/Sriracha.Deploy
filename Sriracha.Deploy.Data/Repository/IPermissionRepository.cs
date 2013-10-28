using Sriracha.Deploy.Data.Dto.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IPermissionRepository
	{
		DeployProjectRole GetProjectRole(string projectRoleId);
		List<DeployProjectRole> GetProjectRoleList(string projectId);
		DeployProjectRole CreateProjectRole(string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator);
		DeployProjectRole UpdateProjectRole(string roleId, string projectId, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator);
		DeployProjectRole TryGetProjectEveryoneRole(string projectId);
	}
}
