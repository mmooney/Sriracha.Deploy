using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.SemanticComparison;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository.Tests.RavenDB;
using Sriracha.Deploy.RavenDB;

namespace Sriracha.Deploy.Data.Repository.Tests
{
    public class BuildRepositoryTests
    {
		private Fixture _fixture = new Fixture();

		[Test]
		[TestCase(typeof(RavenDBRepositoryTypeDefintion<RavenBuildRepository>))]
		public void CreateAndGetBuildList(Type type)
		{
			using(var repositoryType = (RepositoryTypeDefinition)Activator.CreateInstance(type))
			{
				var sut = (IBuildRepository)repositoryType.CreateRepository();
			
				var initialList = sut.GetBuildList();
				Assert.IsNotNull(initialList);

				var newItem1 = sut.CreateBuild(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());
				var newItem2 = sut.CreateBuild(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());

				var newList = sut.GetBuildList();
				Assert.AreEqual(initialList.Count()+2, newList.Count());
			
				var reloadedNewItem1 = newList.SingleOrDefault(i=>i.Id == newItem1.Id);
				Assert.IsNotNull(reloadedNewItem1);

				var reloadedNewItem2 = newList.SingleOrDefault(i=>i.Id == newItem2.Id);
				Assert.IsNotNull(reloadedNewItem2);
			}
		}

		[Test]
		[TestCase(typeof(RavenDBRepositoryTypeDefintion<RavenBuildRepository>))]
		public void CreatePopulatesFields(Type type)
		{
			using(var repositoryType = (RepositoryTypeDefinition)Activator.CreateInstance(type))
			{
				var sut = (IBuildRepository)repositoryType.CreateRepository();

				string projectId = _fixture.Create<string>();
				string projectName = _fixture.Create<string>();
				string projectComponentId = _fixture.Create<string>();
				string projectComponentName = _fixture.Create<string>();
				string projectBranchId = _fixture.Create<string>();
				string projectBranchName = _fixture.Create<string>();
				string fileId = _fixture.Create<string>();
				string version = _fixture.Create<string>();

				var newItem = sut.CreateBuild(projectId, projectName, projectComponentId, projectComponentName, projectBranchId, projectBranchName, fileId, version);

				Assert.IsNotNullOrEmpty(newItem.Id);
				Assert.AreEqual(projectId, newItem.ProjectId);
				Assert.AreEqual(projectName, newItem.ProjectName);
				Assert.AreEqual(projectComponentId, newItem.ProjectComponentId);
				Assert.AreEqual(projectComponentName, newItem.ProjectComponentName);
				Assert.AreEqual(projectBranchId, newItem.ProjectBranchId);
				Assert.AreEqual(projectBranchName, newItem.ProjectBranchName);
				Assert.AreEqual(fileId, newItem.FileId);
				Assert.AreEqual(version, newItem.Version);
			}
		}


		[TestCase(typeof(RavenDBRepositoryTypeDefintion<RavenBuildRepository>))]
		public void UpdateAndGetBuildList(Type type)
		{
			using(var repositoryType = (RepositoryTypeDefinition)Activator.CreateInstance(type))
			{
				var sut = (IBuildRepository)repositoryType.CreateRepository();

				var newItem = sut.CreateBuild(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());

				var initialList = sut.GetBuildList();
				Assert.IsNotNull(initialList);
				Assert.IsNotNull(initialList.SingleOrDefault(i=>i.Id == newItem.Id));

				var updatedItem = sut.UpdateBuild(newItem.Id, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());

				var newList = sut.GetBuildList();
				Assert.AreEqual(initialList.Count(), newList.Count());

				var reloadedItem = newList.SingleOrDefault(i => i.Id == newItem.Id);
				Assert.IsNotNull(reloadedItem);
				var expectedReloadedItem = new Likeness<DeployBuild>(updatedItem);
				Assert.That(expectedReloadedItem.Equals(reloadedItem));
			}
		}
	}
}
