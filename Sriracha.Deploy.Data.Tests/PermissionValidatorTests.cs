﻿using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Account;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests
{
	public class PermissionValidatorTests
	{
		public class GetUserEffectivePermissions
		{
            private void AssertPermission(List<DeployProjectEffectivePermissions> projectPermissionList, EnumPermissionAccess access)
			{
				foreach(var projectPermission in projectPermissionList)
				{
					AssertPermission(projectPermission, access);
				}
			}

			private void AssertPermission(DeployProjectEffectivePermissions projectPermissions, EnumPermissionAccess access)
			{
				Assert.AreEqual(access, projectPermissions.EditComponentConfigurationAccess);
				Assert.AreEqual(access, projectPermissions.CreateEnvironmentAccess);
				Assert.AreEqual(access, projectPermissions.EditProjectPermissionsAccess);

				AssertPermission(projectPermissions.ApproveRejectDeployPermissionList, access);
				AssertPermission(projectPermissions.EditEnvironmentPermissionList, access);
				AssertPermission(projectPermissions.EditEnvironmentPermissionsPermissionList, access);
				AssertPermission(projectPermissions.RequestDeployPermissionList, access);
				AssertPermission(projectPermissions.RunDeploymentPermissionList, access);
			}

			private void AssertPermission(List<DeployProjectRoleEnvironmentPermission> permissionList, EnumPermissionAccess access)
			{
				foreach(var item in permissionList)
				{
					AssertPermission(item, access);
				}
			}

			private void AssertPermission(DeployProjectRoleEnvironmentPermission item, EnumPermissionAccess access)
			{
				Assert.AreEqual(access, item.Access);
			}

			private class TestData
			{
				public Fixture Fixture { get; set; }
				public string UserName { get; set; }
				public Mock<IProjectRoleManager> ProjectRoleManager { get; set; }
                public Mock<ISystemRoleManager> SystemRoleManager { get; set; }
				public Mock<IUserIdentity> UserIdentity { get; set; }
				public List<DeployProjectRole> DeployProjectRoleList { get; set; }
				public List<DeployProject> ProjectList { get; set; }
				public IPermissionValidator Sut { get; set; }

				public static TestData Create(int projectCount=5)
				{
					var fixture = new Fixture();
					var returnValue = new TestData
					{
						Fixture = fixture,
						UserName = fixture.Create<string>("UserName"),
						ProjectList = fixture.CreateMany<DeployProject>(projectCount).ToList(),
						ProjectRoleManager = new Mock<IProjectRoleManager>(),
                        SystemRoleManager = new Mock<ISystemRoleManager>(),
						UserIdentity = new Mock<IUserIdentity>()
					};
					returnValue.DeployProjectRoleList = 
						(from i in returnValue.ProjectList
						 select new DeployProjectRole
							{
								ProjectId = i.Id,
								RoleName = fixture.Create<string>("RoleName")
							}
						).ToList();
                    returnValue.ProjectRoleManager.Setup(i => i.GetProjectRoleListForUser(It.IsAny<string>())).Returns(new List<DeployProjectRole>());
                    returnValue.ProjectRoleManager.Setup(i => i.GetProjectRoleListForUser(returnValue.UserName)).Returns(returnValue.DeployProjectRoleList);

                    returnValue.SystemRoleManager.Setup(i=>i.GetSystemRoleListForUser(It.IsAny<string>())).Returns(new List<SystemRole>());

					returnValue.UserIdentity.Setup(i=>i.UserName).Returns(returnValue.UserName);

					returnValue.Sut = new PermissionValidator(returnValue.ProjectRoleManager.Object, returnValue.SystemRoleManager.Object, returnValue.UserIdentity.Object);

					return returnValue;
				}

				public DeployProjectRole AddRoleAssignment(DeployProject project, DeployEnvironment environment, 
							EnumPermissionAccess createEnvironmentAccess=EnumPermissionAccess.None,
							EnumPermissionAccess editComponentConfigurationAccess=EnumPermissionAccess.None, 
							EnumPermissionAccess editProjectPermissionsAccess=EnumPermissionAccess.None,
							EnumPermissionAccess approveRejectDeployAccess=EnumPermissionAccess.None, 
							EnumPermissionAccess requestDeploymentAccess=EnumPermissionAccess.None,
							EnumPermissionAccess runDeploymentmentAccess=EnumPermissionAccess.None,
							EnumPermissionAccess editEnvironmentAccess=EnumPermissionAccess.None,
							EnumPermissionAccess editEnvironmentPermissionsAccess = EnumPermissionAccess.None)
				{
					string roleId = this.Fixture.Create<string>();
					var role = new DeployProjectRole
					{
						Id = roleId,
						ProjectId = project.Id,
						RoleName = this.Fixture.Create<string>("RoleName"),
						Assignments = new DeployProjectRoleAssignments
						{
							UserNameList = new List<string> { this.UserName }
						},
						EveryoneRoleIndicator = false
					};
					role.Permissions.EditComponentConfigurationAccess = editComponentConfigurationAccess;
					role.Permissions.CreateEnvironmentAccess = createEnvironmentAccess;
					role.Permissions.EditProjectPermissionsAccess = editProjectPermissionsAccess;
					this.SetPermission(project, environment, role.Permissions.ApproveRejectDeployPermissionList, approveRejectDeployAccess);
					this.SetPermission(project, environment, role.Permissions.RequestDeployPermissionList, requestDeploymentAccess);
					this.SetPermission(project, environment, role.Permissions.RunDeploymentPermissionList, runDeploymentmentAccess);
					this.SetPermission(project, environment, role.Permissions.EditEnvironmentPermissionList, editEnvironmentAccess);
					this.SetPermission(project, environment, role.Permissions.EditEnvironmentPermissionsPermissionList, editEnvironmentPermissionsAccess);

					this.DeployProjectRoleList.Add(role);

					return role;
				}

				private void SetPermission(DeployProject project, DeployEnvironment environment, List<DeployProjectRoleEnvironmentPermission> list, EnumPermissionAccess access)
				{
					foreach(var x in project.EnvironmentList)
					{
						var item = list.FirstOrDefault(i=>i.EnvironmentId == x.Id);
						if(item == null)
						{
							item = new DeployProjectRoleEnvironmentPermission
							{
                                //Id = this.Fixture.Create<string>(),
                                //ProjectId = project.Id,
								EnvironmentId = x.Id,
								EnvironmentName = x.CreatedByUserName
							};
							list.Add(item);
						}
					}
					list.First(i=>i.EnvironmentId == environment.Id).Access = access;
				}

			}

			[Test]
			public void NoRoles_DefaultsToGrant()
			{
				var testData = TestData.Create();

				var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

				Assert.IsNotNull(result);
				Assert.AreEqual(testData.UserName, result.UserName);
				Assert.IsNotNull(result.ProjectPermissionList);
				Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
				AssertPermission(result.ProjectPermissionList, EnumPermissionAccess.Grant);
			}

			public class EditProjectPermissions
			{
				[Test]
				public void EditProjectPermissionsToRole_DefaultsGrant()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.EditProjectPermissionsAccess);
				}


				[Test]
				public void GrantEditProjectPermissionsToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editProjectPermissionsAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.EditProjectPermissionsAccess);
				}

				[Test]
				public void DenyEditProjectPermissionsToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editProjectPermissionsAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditProjectPermissionsAccess);
				}

				[Test]
				public void GrantAndDenyEditProjectPermissionsToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editProjectPermissionsAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, editProjectPermissionsAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditProjectPermissionsAccess);
				}
			}

			public class CreateEnvironment
			{
				[Test]
				public void CreateEnvironmentToRole_DefaultsGrant()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.CreateEnvironmentAccess);
				}


				[Test]
				public void GrantCreateEnvironmentToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, createEnvironmentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.CreateEnvironmentAccess);
				}

				[Test]
				public void DenyCreateEnvironmentToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, createEnvironmentAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.CreateEnvironmentAccess);
				}

				[Test]
				public void GrantAndDenyCreateEnvironmentToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, createEnvironmentAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, createEnvironmentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.CreateEnvironmentAccess);
				}
			}

			public class EditComponentConfiguration
			{ 
				[Test]
				public void EditComponentConfigurationToRole_DefaultsGrant()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.EditComponentConfigurationAccess);
				}


				[Test]
				public void GrantEditComponentConfigurationToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editComponentConfigurationAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.EditComponentConfigurationAccess);
				}

				[Test]
				public void DenyEditComponentConfigurationToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editComponentConfigurationAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditComponentConfigurationAccess);
				}

				[Test]
				public void GrantAndDenyEditComponentConfigurationToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editComponentConfigurationAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, editComponentConfigurationAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditComponentConfigurationAccess);
				}
			}

			public class ApproveRejectPermission
			{
				[Test]
				public void GrantApproveRejectPermissionToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count/2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count/2];
					testData.AddRoleAssignment(project, environment, approveRejectDeployAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i=>i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.ApproveRejectDeployPermissionList.Single(i=>i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void DenyApproveRejectPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, approveRejectDeployAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.ApproveRejectDeployPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void GrantAndDenyApproveRejectPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, approveRejectDeployAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, approveRejectDeployAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.ApproveRejectDeployPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}
			}

			public class RequestDeployPermission
			{
				[Test]
				public void GrantRequestDeployPermissionToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, requestDeploymentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.RequestDeployPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void DenyRequestDeployPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, requestDeploymentAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.RequestDeployPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void GrantAndDenyRequestDeployPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, requestDeploymentAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, requestDeploymentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.RequestDeployPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}
			}

			public class RunDeploymentPermission
			{
				[Test]
				public void GrantRunDeploymentPermissionToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, runDeploymentmentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.RunDeploymentPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void DenyRunDeploymentPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, runDeploymentmentAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.RunDeploymentPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void GrantAndDenyRunDeploymentPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, runDeploymentmentAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, runDeploymentmentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.RunDeploymentPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}
			}

			public class EditEnvironmentPermission
			{
				[Test]
				public void GrantEditEnvironmentPermissionToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editEnvironmentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.EditEnvironmentPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void DenyEditEnvironmentPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editEnvironmentAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditEnvironmentPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void GrantAndDenyEditEnvironmentPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editEnvironmentAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, editEnvironmentAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditEnvironmentPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}
			}

			public class EditEnvironmentPermissionsPermission
			{
				[Test]
				public void GrantEditEnvironmentPermissionsPermissionToRole_GrantsAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editEnvironmentPermissionsAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Grant, projectPermissionItem.EditEnvironmentPermissionsPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void DenyEditEnvironmentPermissionsPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editEnvironmentPermissionsAccess: EnumPermissionAccess.Deny);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditEnvironmentPermissionsPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}

				[Test]
				public void GrantAndDenyEnvironmentEditPermissionsPermissionToRole_DeniesAccess()
				{
					var testData = TestData.Create();
					var project = testData.ProjectList[testData.ProjectList.Count / 2];
					var environment = project.EnvironmentList[project.EnvironmentList.Count / 2];
					testData.AddRoleAssignment(project, environment, editEnvironmentPermissionsAccess: EnumPermissionAccess.Deny);
					testData.AddRoleAssignment(project, environment, editEnvironmentPermissionsAccess: EnumPermissionAccess.Grant);

					var result = testData.Sut.GetUserEffectivePermissions(testData.UserName);

					Assert.IsNotNull(result);
					Assert.AreEqual(testData.UserName, result.UserName);
					Assert.IsNotNull(result.ProjectPermissionList);
					Assert.AreEqual(testData.ProjectList.Count, result.ProjectPermissionList.Count);
					var projectPermissionItem = result.ProjectPermissionList.SingleOrDefault(i => i.ProjectId == project.Id);
					Assert.IsNotNull(projectPermissionItem);
					Assert.AreEqual(EnumPermissionAccess.Deny, projectPermissionItem.EditEnvironmentPermissionsPermissionList.Single(i => i.EnvironmentId == environment.Id).Access);
				}
			}
		}

	}
}
