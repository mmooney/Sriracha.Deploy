using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository
{
    public abstract class RazorTemplateRepositoryTests : RepositoryTestBase<IRazorTemplateRepository>
    {
        private void AssertCreatedView(string expectedViewData, string expectedViewName, RazorTemplate actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsNotNullOrEmpty(actual.Id);
            Assert.AreEqual(expectedViewName, actual.ViewName);
            Assert.AreEqual(expectedViewData, actual.ViewData);
            AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
        }

        private void AssertView(RazorTemplate expected, RazorTemplate actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsNotNullOrEmpty(actual.Id);
            Assert.AreEqual(expected.ViewData, actual.ViewData);
            Assert.AreEqual(expected.ViewName, actual.ViewName);
            AssertHelpers.AssertBaseDto(expected, actual);
        }

        private void AssertUpdatedView(RazorTemplate original, string newViewData, RazorTemplate actual)
        {
            Assert.IsNotNull(actual);
            AssertHelpers.AssertUpdatedBaseDto(original, actual, this.UserName);
            Assert.AreEqual(newViewData, actual.ViewData);
        }

        [Test]
        public void GetTemplate_GetsTemplate()
        {
            var sut = this.GetRepository();
            string viewName = this.Fixture.Create<string>("ViewName");
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");
            sut.SaveTemplate(viewName, viewData);

            var result = sut.GetTemplate(viewName, defaultViewData);

            AssertCreatedView(viewData, viewName, result);
        }

        [Test]
        public void GetTemplate_MissingViewName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            string viewName = null;
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");

            Assert.Throws<ArgumentNullException>(()=>sut.GetTemplate(viewName, defaultViewData));
        }

        [Test]
        public void GetTemplate_UnknownViewName_ReturnsDefault()
        {
            var sut = this.GetRepository();
            string viewName = this.Fixture.Create<string>("ViewName");
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");

            var result = sut.GetTemplate(viewName, defaultViewData);

            Assert.AreEqual(defaultViewData, result.ViewData);
        }

        [Test]
        public void SaveTemplate_SavesNewTemplate()
        {
            var sut = this.GetRepository();
            string viewName = this.Fixture.Create<string>("ViewName");
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");
            
            var result = sut.SaveTemplate(viewName, viewData);
            AssertCreatedView(viewData, viewName, result);
            var dbItem = sut.GetTemplate(viewName, defaultViewData);
            AssertView(result, dbItem);
        }

        [Test]
        public void SaveTemplate_UpdatesExistingTemplate()
        {
            var sut = this.GetRepository();
            string viewName = this.Fixture.Create<string>("ViewName");
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");
            var original = sut.SaveTemplate(viewName, viewData);
            string newViewData = this.Fixture.Create<string>("NewViewData");
            this.CreateNewUserName();

            var result = sut.SaveTemplate(viewName, newViewData);

            AssertUpdatedView(original, newViewData, result);
            var dbItem = sut.GetTemplate(viewName, defaultViewData);
            AssertView(result, dbItem);
        }

        [Test]
        public void SaveTemplate_MissingViewName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            string viewName = null;
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");

            Assert.Throws<ArgumentNullException>(() => sut.SaveTemplate(viewName, viewData));
        }

        [Test]
        public void SaveTemplate_MissingViewData_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            string viewName = this.Fixture.Create<string>("ViewName");
            string viewData = null;
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");

            Assert.Throws<ArgumentNullException>(() => sut.SaveTemplate(viewName, viewData));
        }

        [Test]
        public void DeleteTemplate_DeletesTemplate()
        {
            var sut = this.GetRepository();
            string viewName = this.Fixture.Create<string>("ViewName");
            string viewData = this.Fixture.Create<string>("ViewData");
            string defaultViewData = this.Fixture.Create<string>("DefaultViewData");
            var template = sut.SaveTemplate(viewName, viewData);

            var result = sut.DeleteTemplate(viewName);

            Assert.IsNotNull(result);
            Assert.AreEqual(template.Id, result.Id);;
            var dbItem = sut.GetTemplate(viewName, defaultViewData);
            Assert.AreEqual(defaultViewData, dbItem.ViewData);
        }

        [Test]
        public void DeleteTemplate_MissingViewName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.DeleteTemplate(null));
        }

        [Test]
        public void DeleteTemplate_BadViewName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteTemplate(Guid.NewGuid().ToString()));
        }
    }
}
