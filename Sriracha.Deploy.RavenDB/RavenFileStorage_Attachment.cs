using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenFileStorage_Attachment : IFileStorage
	{
		private readonly IRavenAttachmentManager _ravenAttachmentManager;

		public RavenFileStorage_Attachment(IRavenAttachmentManager ravenAttachmentManager)
		{
			_ravenAttachmentManager = DIHelper.VerifyParameter(ravenAttachmentManager);
		}

		public string StoreFile(byte[] fileData)
		{
			string attachmentId = Guid.NewGuid().ToString();
			_ravenAttachmentManager.SetAttachment(attachmentId, fileData);
			return attachmentId;
		}

		public void UpdateFile(string fileStorageId, byte[] fileData)
		{
			throw new NotImplementedException();
		}


		public byte[] GetFile(string fileStorageId)
		{
			return _ravenAttachmentManager.GetAttachment(fileStorageId);
		}
	}
}
