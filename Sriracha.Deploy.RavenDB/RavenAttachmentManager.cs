using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Client;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenAttachmentManager : IRavenAttachmentManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public RavenAttachmentManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public string GetAttachmentString(string attachmentId)
		{
			var attachment = this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachment(attachmentId);
			if(attachment == null)
			{
				throw new Exception(string.Format("Attachment {0} not found", attachmentId));
			}
			using (var reader = new StreamReader(attachment.Data()))
			{
				return reader.ReadToEnd();
			}

		}

		public void SetAttachment(string attachmentId, byte[] fileData)
		{
			using (var stream = new MemoryStream())
			{
				stream.Write(fileData,0,fileData.Length);
				stream.Position = 0;
				this.SetAttachment(attachmentId, stream);
			}
		}

		public void SetAttachment(string attachmentId, string attachmentData)
		{
			using (var stream = new MemoryStream())
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(attachmentData);
				writer.Flush();
				stream.Position = 0;
				this.SetAttachment(attachmentId, stream);
			}
		}

		public void SetAttachment(string attachmentId, Stream stream)
		{
			this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.PutAttachment(attachmentId, null, stream, new Raven.Json.Linq.RavenJObject());
		}

        public byte[] GetAttachment(string attachmentId)
        {
            var attachment = this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachment(attachmentId);
            if (attachment == null)
            {
                throw new Exception("Attachment Not Found: " + attachmentId);
            }
            using (var memoryStream = new MemoryStream())
            {
                attachment.Data().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public Stream GetAttachmentStream(string attachmentId)
        {
            var attachment = this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachment(attachmentId);
            if (attachment == null)
            {
                throw new Exception("Attachment Not Found: " + attachmentId);
            }
            return attachment.Data();
        }

        public IEnumerable<Raven.Abstractions.Data.Attachment> GetAttachmentList()
		{
			return this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachmentHeadersStartingWith("",0,int.MaxValue).ToList();
		}


		public void RemoveAttachment(string attachmentId)
		{
			this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.DeleteAttachment(attachmentId, null);
		}
	}
}
