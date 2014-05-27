using NUnit.Framework;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Account
{
    public abstract class PermissionRepositoryTests : RepositoryTestBase<IPermissionRepository>
    {
        private void AssertRole(DeployProjectRole expected, DeployProjectRole actual)
        {
            Assert.IsNotNull(actual);
            AssertHelpers.AssertBaseDto(expected, actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ProjectName, actual.ProjectName);
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.EveryoneRoleIndicator, actual.EveryoneRoleIndicator);

            AssertPermissions(expected.Permissions, actual.Permissions);

            Assert.IsNotNull(actual.Assignments);
            AssertHelpers.AssertStringList(expected.Assignments.UserNameList, actual.Assignments.UserNameList);
        }

        private void AssertPermissions(DeployProjectRolePermissions expected, DeployProjectRolePermissions actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.EditComponentConfigurationAccess, actual.EditComponentConfigurationAccess);
            Assert.AreEqual(expected.CreateEnvironmentAccess, actual.CreateEnvironmentAccess);
            Assert.AreEqual(expected.EditProjectPermissionsAccess, actual.EditProjectPermissionsAccess);
            AssertPermissionList(expected.RequestDeployPermissionList, actual.RequestDeployPermissionList);
            AssertPermissionList(expected.ApproveRejectDeployPermissionList, actual.ApproveRejectDeployPermissionList);
            AssertPermissionList(expected.RunDeploymentPermissionList, actual.RunDeploymentPermissionList);
            AssertPermissionList(expected.EditEnvironmentPermissionList, actual.EditEnvironmentPermissionList);
            AssertPermissionList(expected.EditEnvironmentPermissionsPermissionList, actual.EditEnvironmentPermissionsPermissionList);
        }

        private void AssertUpdatedRole(DeployProjectRole original, DeployProjectRole expected, DeployProjectRole actual)
        {
            AssertHelpers.AssertUpdatedBaseDto(original, actual, this.UserName);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ProjectName, actual.ProjectName);
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.EveryoneRoleIndicator, actual.EveryoneRoleIndicator);

            AssertPermissions(expected.Permissions, actual.Permissions);

            Assert.IsNotNull(actual.Assignments);
            AssertHelpers.AssertStringList(expected.Assignments.UserNameList, actual.Assignments.UserNameList);
        }

        private void AssertCreatedRole(DeployProjectRole expected, DeployProjectRole actual)
        {
            AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ProjectName, actual.ProjectName);
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.EveryoneRoleIndicator, actual.EveryoneRoleIndicator);

            AssertPermissions(expected.Permissions, actual.Permissions);

            Assert.IsNotNull(actual.Assignments);
            AssertHelpers.AssertStringList(expected.Assignments.UserNameList, actual.Assignments.UserNameList);
        }

        private void AssertPermissionList(List<DeployProjectRoleEnvironmentPermission> expectedList, List<DeployProjectRoleEnvironmentPermission> actualList)
        {
            if(expectedList == null)
            {
                Assert.IsNull(actualList);
            }
            else 
            {
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedList.Count, actualList.Count);
                foreach(var expectedItem in expectedList)
                {
                    var actualItem = actualList.FirstOrDefault(i=>i.EnvironmentId == expectedItem.EnvironmentId);
                    Assert.IsNotNull(actualItem);
                    Assert.AreEqual(expectedItem.EnvironmentName, actualItem.EnvironmentName);
                    Assert.AreEqual(expectedItem.Access, actualItem.Access);
                }
            }
        }

        [Test]
        public void GetProjectRole_GetsProjectRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();
            var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);

            var result = sut.GetProjectRole(role.Id);
            AssertRole(role, result);
        }

        [Test]
        public void GetProjectRole_MissingRoleID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            
            Assert.Throws<ArgumentNullException>(()=>sut.GetProjectRole(null));
        }

        [Test]
        public void GetProjectRole_BadRoleID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetProjectRole(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetProjectRoleList_GetsProjectRoleList()
        {
            var sut = this.GetRepository();
            var projectId = this.Fixture.Create<string>();
            var projectRoleList = new List<DeployProjectRole>();
            for(var i = 0; i < 8; i++)
            {
                var data = this.Fixture.Create<DeployProjectRole>();
                var role = sut.CreateProjectRole(projectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);
                projectRoleList.Add(role);
            }
            var otherRoleList = new List<DeployProjectRole>();
            for (var i = 0; i < 9; i++)
            {
                var data = this.Fixture.Create<DeployProjectRole>();
                var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);
                otherRoleList.Add(role);
            }

            var result = sut.GetProjectRoleList(projectId);

            Assert.IsNotNull(result);
            Assert.AreEqual(projectRoleList.Count, result.Count);
            foreach(var expectedItem in projectRoleList)
            {
                var actualItem = result.FirstOrDefault(i=>i.Id == expectedItem.Id);
                AssertRole(expectedItem, actualItem);
            }
        }

        [Test]
        public void GetProjectRoleList_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetProjectRoleList(null));
        }

        [Test]
        public void GetProjectRoleList_UnknownProjectID_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            var result = sut.GetProjectRoleList(Guid.NewGuid().ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetProjectRoleListForUser_GetsProjectRoleListForUser()
        {
            var sut = this.GetRepository();
            var userName = this.Fixture.Create<string>();
            var projectRoleList = new List<DeployProjectRole>();
            for (var i = 0; i < 8; i++)
            {
                var data = this.Fixture.Create<DeployProjectRole>();
                data.Assignments.UserNameList.Add(userName);
                var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);
                projectRoleList.Add(role);
            }
            var otherRoleList = new List<DeployProjectRole>();
            for (var i = 0; i < 9; i++)
            {
                var data = this.Fixture.Create<DeployProjectRole>();
                var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);
                otherRoleList.Add(role);
            }

            var result = sut.GetProjectRoleListForUser(userName);

            Assert.IsNotNull(result);
            Assert.AreEqual(projectRoleList.Count, result.Count);
            foreach (var expectedItem in projectRoleList)
            {
                var actualItem = result.FirstOrDefault(i => i.Id == expectedItem.Id);
                AssertRole(expectedItem, actualItem);
            }
        }

        [Test]
        public void GetProjectRoleListForUser_MissingUserName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetProjectRoleListForUser(null));
        }

        [Test]
        public void GetProjectRoleListForUser_UnknownUserName_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            var result = sut.GetProjectRoleListForUser(Guid.NewGuid().ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void CreateProjectRole_CreatesProjectRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();

            var result = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);

            AssertCreatedRole(data, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void CreateProjectRole_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();

            Assert.Throws<ArgumentNullException>(()=>sut.CreateProjectRole(null, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator));
        }

        [Test]
        public void CreateProjectRole_MissingProjectName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();

            Assert.Throws<ArgumentNullException>(() => sut.CreateProjectRole(data.ProjectId, null, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator));
        }

        [Test]
        public void CreateProjectRole_MissingRoleName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();

            Assert.Throws<ArgumentNullException>(() => sut.CreateProjectRole(data.ProjectId, data.ProjectName, null, data.Permissions, data.Assignments, data.EveryoneRoleIndicator));
        }


        [Test]
        public void CreateProjectRole_MissingPermissions_CreatesProjectRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();

            var result = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, null, data.Assignments, data.EveryoneRoleIndicator);

            data.Permissions = new DeployProjectRolePermissions();
            AssertCreatedRole(data, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void CreateProjectRole_Missingssignments_CreatesProjectRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();

            var result = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, null, data.EveryoneRoleIndicator);

            data.Assignments = new DeployProjectRoleAssignments();
            AssertCreatedRole(data, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void CreateProjectRole_DuplicateProjectIDRoleName_ThrowsArgumentException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.RoleName = data1.RoleName;
            data2.ProjectId = data1.ProjectId;

            Assert.Throws<ArgumentException>(()=>sut.CreateProjectRole(data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator));
        }

        [Test]
        public void CreateProjectRole_DuplicateRoleNameDifferentProjectID_CreatesProjectRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.RoleName = data1.RoleName;

            var result = sut.CreateProjectRole(data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator);

            AssertCreatedRole(data2, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void UpdateProjectRole_UpdatesProjectRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            var result = sut.UpdateProjectRole(data2.Id, data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator);

            AssertUpdatedRole(role1, data2, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void UpdateProjectRole_MissingProjectRoleID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateProjectRole(null, data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator));
        }

        [Test]
        public void UpdateProjectRole_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(() => sut.UpdateProjectRole(data2.Id, null, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator));
        }


        [Test]
        public void UpdateProjectRole_MissingProjectName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(() => sut.UpdateProjectRole(data2.Id, data2.ProjectId, null, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator));
        }

        [Test]
        public void UpdateProjectRole_MissingRoleName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(() => sut.UpdateProjectRole(data2.Id, data2.ProjectId, data2.ProjectName, null, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator));
        }

        [Test]
        public void UpdateProjectRole_NullAssignments_UpdatesProjectRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            var result = sut.UpdateProjectRole(data2.Id, data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, null, data2.EveryoneRoleIndicator);

            data2.Assignments = new DeployProjectRoleAssignments();
            AssertUpdatedRole(role1, data2, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void UpdateProjectRole_NullPermissions_UpdatesProjectRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            data2.Id = role1.Id;
            this.CreateNewUserName();

            var result = sut.UpdateProjectRole(data2.Id, data2.ProjectId, data2.ProjectName, data2.RoleName, null, data2.Assignments, data2.EveryoneRoleIndicator);

            data2.Permissions = new DeployProjectRolePermissions();
            AssertUpdatedRole(role1, data2, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void UpdateProjectRole_DuplicateProjectIDAndRoleName_ThrowsArgumentException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            var role2 = sut.CreateProjectRole(data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator);
            var data3 = this.Fixture.Create<DeployProjectRole>();
            data3.Id = role2.Id;
            data3.ProjectId = data1.ProjectId;
            data3.RoleName = data1.RoleName;
            this.CreateNewUserName();

            Assert.Throws<ArgumentException>(()=>sut.UpdateProjectRole(data3.Id, data3.ProjectId, data3.ProjectName, data3.RoleName, data3.Permissions, data3.Assignments, data3.EveryoneRoleIndicator));
        }

        [Test]
        public void UpdateProjectRole_SameProjectIDDifferentRoleName_UpdatesRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            var role2 = sut.CreateProjectRole(data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator);
            var data3 = this.Fixture.Create<DeployProjectRole>();
            data3.Id = role2.Id;
            data3.ProjectId = data1.ProjectId;
            this.CreateNewUserName();

            var result = sut.UpdateProjectRole(data3.Id, data3.ProjectId, data3.ProjectName, data3.RoleName, data3.Permissions, data3.Assignments, data3.EveryoneRoleIndicator);

            AssertUpdatedRole(role2, data3, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }

        [Test]
        public void UpdateProjectRole_SameRoleNameDifferentProjectID_UpdatesRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<DeployProjectRole>();
            var role1 = sut.CreateProjectRole(data1.ProjectId, data1.ProjectName, data1.RoleName, data1.Permissions, data1.Assignments, data1.EveryoneRoleIndicator);
            var data2 = this.Fixture.Create<DeployProjectRole>();
            var role2 = sut.CreateProjectRole(data2.ProjectId, data2.ProjectName, data2.RoleName, data2.Permissions, data2.Assignments, data2.EveryoneRoleIndicator);
            var data3 = this.Fixture.Create<DeployProjectRole>();
            data3.Id = role2.Id;
            data3.RoleName = data1.RoleName;
            this.CreateNewUserName();

            var result = sut.UpdateProjectRole(data3.Id, data3.ProjectId, data3.ProjectName, data3.RoleName, data3.Permissions, data3.Assignments, data3.EveryoneRoleIndicator);

            AssertUpdatedRole(role2, data3, result);
            var dbItem = sut.GetProjectRole(result.Id);
            AssertRole(result, dbItem);
        }
        
        [Test]
        public void TryGetProjectEveryoneRole_WithEveryoneRole_ReturnsEveryoneRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();
            data.EveryoneRoleIndicator = true;
            var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);

            var result = sut.TryGetProjectEveryoneRole(data.ProjectId);

            AssertRole(role, result);
        }

        [Test]
        public void TryGetProjectEveryoneRole_WithoutEveryoneRole_ReturnsNull()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();
            data.EveryoneRoleIndicator = false;
            var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);

            var result = sut.TryGetProjectEveryoneRole(data.ProjectId);

            Assert.IsNull(result);
        }

        [Test]
        public void DeleteProjectRole_DeletesProjectRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<DeployProjectRole>();
            var role = sut.CreateProjectRole(data.ProjectId, data.ProjectName, data.RoleName, data.Permissions, data.Assignments, data.EveryoneRoleIndicator);

            var result = sut.DeleteProjectRole(role.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(role.Id, result.Id);
        }

        [Test]
        public void DeleteProjectRole_MissingRoleID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.DeleteProjectRole(null));
        }

        [Test]
        public void DeleteProjectRole_BadRoleID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteProjectRole(Guid.NewGuid().ToString()));
        }
    }
}
