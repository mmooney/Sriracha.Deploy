using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace MMDB.Permissions.Tests
{
	[TestFixture]
	public abstract class PermissionRepositoryBaseTests
	{
		protected abstract IPermissionRepository GetRepository();

		[Test]
		public void CreatePermission()
		{
			var sut = this.GetRepository();
			var existingPermissionList = sut.GetPermissionList();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			Assert.IsNotNull(newPermission);
			Assert.AreEqual(permissionName, newPermission.PermissionName);
			Assert.AreEqual(permissionDisplayValue, newPermission.PermissionDisplayValue);

			var newPermissionList = sut.GetPermissionList();
			Assert.AreEqual(existingPermissionList.Count+1, newPermissionList.Count);
			var newDbItem = newPermissionList.SingleOrDefault(i=>i.Id == newPermission.Id);
			Assert.IsNotNull(newDbItem);
			Assert.AreEqual(permissionName, newDbItem.PermissionName);
			Assert.AreEqual(permissionDisplayValue, newDbItem.PermissionDisplayValue);
		}

		[Test]
		public void CreatePermission_ChecksUniqe()
		{
			var sut = this.GetRepository();
			var existingPermissionList = sut.GetPermissionList();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			string secondPermissionDisplayValue = fixture.Create<string>();
			Assert.Throws<ArgumentException>(()=> sut.CreatePermission(permissionName, secondPermissionDisplayValue));
		}

		[Test]
		public void GetPermission()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			var dbItem = sut.GetPermission(newPermission.Id);
			Assert.IsNotNull(dbItem);
			Assert.AreEqual(permissionName, dbItem.PermissionName);
			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
		}

	}
}
