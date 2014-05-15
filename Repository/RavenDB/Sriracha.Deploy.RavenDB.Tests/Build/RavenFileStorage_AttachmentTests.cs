using NUnit.Framework;
using Raven.Client;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Tests.Repository.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB.Tests.Build
{
    public class RavenFileStorage_AttachmentTests : FileStorageTests
    {
        protected IDocumentSession DocumentSession { get; private set; }
        protected IRavenAttachmentManager RavenAttachmentManager { get; set; }

        [SetUp]
        public void RavenSetUp()
        {
            this.DocumentSession = EmbeddedRavenProvider.DocumentStore.OpenSession();
            this.RavenAttachmentManager = new RavenAttachmentManager(this.DocumentSession);
        }

        protected override IFileStorage GetRepository()
        {
            return new RavenFileStorage_Attachment(this.RavenAttachmentManager);
        }


        [TearDown]
        public void RavenTearDown()
        {
            using (this.DocumentSession) { };
            this.DocumentSession = null;
        }
    }
}
