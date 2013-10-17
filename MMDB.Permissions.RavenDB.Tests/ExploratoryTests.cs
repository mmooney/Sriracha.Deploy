using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace MMDB.Permissions.RavenDB.Tests
{
	public class ExploratoryTests
	{
		[Test, Ignore]
		public void Test1()
		{
			var fixture = new Fixture();
			var project = new
			{
				Id = fixture.Create<string>(),
				CompnentList = new[]
				{
					new { Id = fixture.Create<string>(), ComponentName = fixture.Create<string>() },
					new { Id = fixture.Create<string>(), ComponentName = fixture.Create<string>() },
					new { Id = fixture.Create<string>(), ComponentName = fixture.Create<string>() }
				},
				EnvironmentList = new[]
				{
					new { Id = fixture.Create<string>(), EnvironmentName = fixture.Create<string>() },
					new { Id = fixture.Create<string>(), EnvironmentName = fixture.Create<string>() },
					new { Id = fixture.Create<string>(), EnvironmentName = fixture.Create<string>() }
				}
			};

			using(var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var repository = new RavenDBPermissionRepository(session);
				IPermissionManager sut = new PermissionManager(repository);
				var group = sut.CreateGroup(fixture.Create<string>(), null);
				var approveDeploymentPermissionDefinition = sut.CreatePermissionDefinition("ApproveDeployment", "Approve Deployment");

				var roleDataItems = new List<PermissionDataAssignment>
				{
					new PermissionDataAssignment { Id = fixture.Create<string>(), DataPropertyName="Project", DataPropertyValue = fixture.Create<string>() }
				};
				var role = sut.CreateRole("QA Approvers", roleDataItems);
				RoleGroupAssignment roleGroupAssignment = sut.AssignGroupToRole(role.Id, group.Id);
			}
		}
	}
}
