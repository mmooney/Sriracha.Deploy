using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Account;
using System.Transactions;
using Sriracha.Deploy.Data.Dto;
using MMDB.Shared;
using Sriracha.Deploy.Data.Exceptions;

namespace Sriracha.Deploy.Data.Tests.Repository.Account
{
    public abstract class SystemRoleRepositoryTests : RepositoryTestBase<ISystemRoleRepository>
    {
        private void AssertCreatedSystemRole(SystemRole expected, SystemRole actual)
        {
            AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.RoleType, actual.RoleType);

            Assert.IsNotNull(actual.Permissions);
            Assert.AreEqual(expected.Permissions.EditSystemPermissionsAccess, actual.Permissions.EditSystemPermissionsAccess);
            Assert.AreEqual(expected.Permissions.EditUsersAccess, actual.Permissions.EditUsersAccess);
            Assert.AreEqual(expected.Permissions.EditDeploymentCredentialsAccess, actual.Permissions.EditDeploymentCredentialsAccess);
            Assert.AreEqual(expected.Permissions.EditDeploymentToolsAccess, actual.Permissions.EditDeploymentToolsAccess);
            Assert.AreEqual(expected.Permissions.EditBuildPurgeRulesAccess, actual.Permissions.EditBuildPurgeRulesAccess);

            Assert.IsNotNull(actual.Assignments);
            AssertHelpers.AssertStringList(expected.Assignments.UserNameList, actual.Assignments.UserNameList);
        }

        private void AssertUpdatedSystemRole(SystemRole original, SystemRole expected, SystemRole actual)
        {
            AssertHelpers.AssertUpdatedBaseDto(original, actual, this.UserName);
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.RoleType, actual.RoleType);

            Assert.IsNotNull(actual.Permissions);
            Assert.AreEqual(expected.Permissions.EditSystemPermissionsAccess, actual.Permissions.EditSystemPermissionsAccess);
            Assert.AreEqual(expected.Permissions.EditUsersAccess, actual.Permissions.EditUsersAccess);
            Assert.AreEqual(expected.Permissions.EditDeploymentCredentialsAccess, actual.Permissions.EditDeploymentCredentialsAccess);
            Assert.AreEqual(expected.Permissions.EditDeploymentToolsAccess, actual.Permissions.EditDeploymentToolsAccess);
            Assert.AreEqual(expected.Permissions.EditBuildPurgeRulesAccess, actual.Permissions.EditBuildPurgeRulesAccess);

            Assert.IsNotNull(actual.Assignments);
            AssertHelpers.AssertStringList(expected.Assignments.UserNameList, actual.Assignments.UserNameList);
        }

        private void AssertSystemRole(SystemRole expected, SystemRole actual)
        {
            AssertHelpers.AssertBaseDto(expected, actual);
            Assert.AreEqual(expected.RoleName, actual.RoleName);
            Assert.AreEqual(expected.RoleType, actual.RoleType);

            Assert.IsNotNull(actual.Permissions);
            Assert.AreEqual(expected.Permissions.EditSystemPermissionsAccess, actual.Permissions.EditSystemPermissionsAccess);
            Assert.AreEqual(expected.Permissions.EditUsersAccess, actual.Permissions.EditUsersAccess);
            Assert.AreEqual(expected.Permissions.EditDeploymentCredentialsAccess, actual.Permissions.EditDeploymentCredentialsAccess);
            Assert.AreEqual(expected.Permissions.EditDeploymentToolsAccess, actual.Permissions.EditDeploymentToolsAccess);
            Assert.AreEqual(expected.Permissions.EditBuildPurgeRulesAccess, actual.Permissions.EditBuildPurgeRulesAccess);

            Assert.IsNotNull(actual.Assignments);
            AssertHelpers.AssertStringList(expected.Assignments.UserNameList, actual.Assignments.UserNameList);
        }

        private void DeleteAll(ISystemRoleRepository sut)
        {
            bool done = false;
            while(!done)
            {
                var result = sut.GetSystemRoleList(new ListOptions { PageSize=int.MaxValue });
                if(result.Items.Count > 0)
                {
                    foreach(var item in result.Items)
                    {
                        sut.DeleteSystemRole(item.Id);
                    }
                }
                else 
                {
                    done = true;
                }
            }
        }

        [Test]
        public void CreateSystemRole_CreatesSystemRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();

            var result = sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);

            Assert.IsNotNull(result);
            AssertCreatedSystemRole(data, result);
            var dbItem = sut.GetSystemRole(result.Id);
            AssertSystemRole(dbItem, result);
        }

        [Test]
        public void CreateSystemRole_MissingRoleName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();

            Assert.Throws<ArgumentNullException>(()=>sut.CreateSystemRole(null, data.RoleType, data.Permissions, data.Assignments));
        }

        [Test]
        public void CreateSystemRole_MissingPermissions_CreatesPermissions()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();

            var result = sut.CreateSystemRole(data.RoleName, data.RoleType, null, data.Assignments);

            data.Permissions = new SystemRolePermissions();
            AssertCreatedSystemRole(data, result);
            var dbItem = sut.GetSystemRole(result.Id);
            AssertSystemRole(dbItem, result);
        }

        [Test]
        public void CreateSystemRole_MissingAssignments_CreatesAssignments()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();

            var result = sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, null);

            data.Assignments = new SystemRoleAssignments();
            AssertCreatedSystemRole(data, result);
            var dbItem = sut.GetSystemRole(result.Id);
            AssertSystemRole(dbItem, result);
        }

        [Test]
        public void CreateSystemRole_DuplicateRoleName_ThrowsArgumentException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<SystemRole>();
            var role1 = sut.CreateSystemRole(data1.RoleName, data1.RoleType, data1.Permissions, data1.Assignments);
            var data2 = this.Fixture.Create<SystemRole>();

            Assert.Throws<ArgumentException>(()=>sut.CreateSystemRole(data1.RoleName, data2.RoleType, data2.Permissions, data2.Assignments));
        }

        [Test]
        public void GetSystemRole_GetsSystemRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();
            var role = sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);

            var result = sut.GetSystemRole(role.Id);
            AssertSystemRole(role, result);
        }

        [Test]
        public void GetSystemRole_MissingID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetSystemRole(null));
        }

        [Test]
        public void GetSystemRole_BadID_RecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetSystemRole(Guid.NewGuid().ToString()));
        }

        [Test]
        public void UpdateSystemRole_UpdatesSystemRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<SystemRole>();
            var role = sut.CreateSystemRole(data1.RoleName, data1.RoleType, data1.Permissions, data1.Assignments);
            var data2 = this.Fixture.Create<SystemRole>();

            var result = sut.UpdateSystemRole(role.Id, data2.RoleName, data2.RoleType, data2.Permissions, data2.Assignments);
            
            AssertUpdatedSystemRole(role, data2, result);
            var dbItem = sut.GetSystemRole(role.Id);
            AssertSystemRole(result, dbItem);
        }

        [Test]
        public void UpdateSystemRole_MissingRoleName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateSystemRole(null, data.RoleName, data.RoleType, data.Permissions, data.Assignments));
        }

        [Test]
        public void UpdateSystemRole_SameRoleSameRoleName_UpdateSystemRole()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<SystemRole>();
            var role = sut.CreateSystemRole(data1.RoleName, data1.RoleType, data1.Permissions, data1.Assignments);
            var data2 = this.Fixture.Create<SystemRole>();
            data2.RoleName = data1.RoleName;

            var result = sut.UpdateSystemRole(role.Id, data2.RoleName, data2.RoleType, data2.Permissions, data2.Assignments);

            AssertUpdatedSystemRole(role, data2, result);
            var dbItem = sut.GetSystemRole(role.Id);
            AssertSystemRole(result, dbItem);
        }

        [Test]
        public void UpdateSystemRole_DifferentRoleSameRoleName_ThrowsArgumentException()
        {
            var sut = this.GetRepository();
            var data1 = this.Fixture.Create<SystemRole>();
            var role1 = sut.CreateSystemRole(data1.RoleName, data1.RoleType, data1.Permissions, data1.Assignments);
            var data2 = this.Fixture.Create<SystemRole>();
            var role2 = sut.CreateSystemRole(data2.RoleName, data2.RoleType, data2.Permissions, data2.Assignments);

            Assert.Throws<ArgumentException>(()=>sut.UpdateSystemRole(role1.Id, role2.RoleName, role2.RoleType, role1.Permissions, role1.Assignments));
        }

        [Test]
        public void TryGetSpecialSystemRole_GetsSpecialSystemRole()
        {
            var sut = this.GetRepository();
            //using(var transaction = new TransactionScope())
            //{
                this.DeleteAll(sut);
                var data = this.Fixture.Create<SystemRole>();
                var role = sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);

                var result = sut.TryGetSpecialSystemRole(data.RoleType);

                AssertSystemRole(role, result);
            //}
        }

        [Test]
        public void TryGetSpecialSystemRole_NoRole_GetsSpecialSystemRole()
        {
            var sut = this.GetRepository();
            this.DeleteAll(sut);
            var data = this.Fixture.Create<SystemRole>();

            var result = sut.TryGetSpecialSystemRole(data.RoleType);

            Assert.IsNull(result);
        }

        [Test]
        public void GetSystemRoleListForUser_GetsRolesForUser()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();
            var userName = data.Assignments.UserNameList.First();
            var role = sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);

            var result = sut.GetSystemRoleListForUser(userName);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            AssertSystemRole(role, result[0]);
        }

        [Test]
        public void GetSystemRoleListForUser_UnknownUserName_ReturnsEmptyList()
        {
            var sut = this.GetRepository();
            var userName = this.Fixture.Create<string>("UserName");

            var result = sut.GetSystemRoleListForUser(userName);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void DeleteSystemRole_DeletesSystemRole()
        {
            var sut = this.GetRepository();
            var data = this.Fixture.Create<SystemRole>();
            var role = sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);

            var result = sut.DeleteSystemRole(role.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(role.Id, result.Id);
            Assert.Throws<RecordNotFoundException>(() => sut.GetSystemRole(role.Id));
        }

        [Test]
        public void DeleteSystemRole_MissingID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            
            Assert.Throws<ArgumentNullException>(()=>sut.DeleteSystemRole(null));
        }

        [Test]
        public void DeleteSystemRole_BadID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteSystemRole(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetSystemRoleList_GetsSystemRoleList()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 10; i++)
            {
                var data = this.Fixture.Create<SystemRole>();
                sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);
            }

            var result = sut.GetSystemRoleList(null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreNotEqual(0, result.Items.Count);
            Assert.AreNotEqual(0, result.PageSize);
        }

        [Test]
        public void GetSystemRoleList_Defaults()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SystemRole>();
                sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);
            }

            var result = sut.GetSystemRoleList(null);

            int defaultPageSize = 20;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(defaultPageSize, result.Items.Count);
            Assert.AreEqual(defaultPageSize, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(2, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsTrue(result.SortAscending);
            Assert.AreEqual("RoleName", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetSystemList_PageSize()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SystemRole>();
                sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);
            }

            var result = sut.GetSystemRoleList(new ListOptions { PageSize = 5 });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(5, result.Items.Count);
            Assert.AreEqual(5, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(6, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsTrue(result.SortAscending);
            Assert.AreEqual("RoleName", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetSystemRoleList_SortByUserNameDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SystemRole>();
                sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);
            }

            var result = sut.GetSystemRoleList(new ListOptions { SortField = "RoleName", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            SystemRole lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.RoleName, item.RoleName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetSystemRoleList_SortByUpdatedDateTimeUtcAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                var data = this.Fixture.Create<SystemRole>();
                sut.CreateSystemRole(data.RoleName, data.RoleType, data.Permissions, data.Assignments);
            }

            var result = sut.GetSystemRoleList(new ListOptions { SortField = "RoleName", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            SystemRole lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.RoleName, item.RoleName);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetSystemRoleList_BadSortField_ThrowsUnrecognizedSortFieldException()
        {
            var sut = this.GetRepository();

            Assert.Throws<UnrecognizedSortFieldException<SystemRole>>(() => sut.GetSystemRoleList(new ListOptions { SortField = Guid.NewGuid().ToString() }));
        }
    }
}
