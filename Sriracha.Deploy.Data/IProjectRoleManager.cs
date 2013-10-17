using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IProjectRoleManager
	{
		DeployProjectRole GetProjectRole(string projectRoleId);
		List<DeployProjectRole> GetProjectRoleList(string projectId);

		DeployProjectRole CreateRole(string projectId, string roleName);
		DeployProjectRole UpdateRole(string roleId, string projectId, string roleName);
	}
}
