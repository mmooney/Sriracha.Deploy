using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Build;
using MMDB.Shared;

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
            if(fileData == null || fileData.Length == 0)
            {
                throw new ArgumentNullException("fileData");
            }
			string attachmentId = Guid.NewGuid().ToString();
			_ravenAttachmentManager.SetAttachment(attachmentId, fileData);
			return attachmentId;
		}


		public void UpdateFile(string fileStorageId, byte[] fileData)
		{
            if(string.IsNullOrEmpty(fileStorageId))
            {
                throw new ArgumentNullException("fileStorageId");
            }
			if(!_ravenAttachmentManager.AttachmentExists(fileStorageId))
            {
                throw new RecordNotFoundException(typeof(byte[]), "fileStorageId", fileStorageId);
            }
            _ravenAttachmentManager.SetAttachment(fileStorageId, fileData);
		}


		public byte[] GetFile(string fileStorageId)
		{
			return _ravenAttachmentManager.GetAttachment(fileStorageId);
		}


        public Stream GetFileStream(string fileStorageId)
        {
            return _ravenAttachmentManager.GetAttachmentStream(fileStorageId);
        }
        
        public void DeleteFile(string fileStorageId)
		{
			_ravenAttachmentManager.RemoveAttachment(fileStorageId);
		}
    }
}
