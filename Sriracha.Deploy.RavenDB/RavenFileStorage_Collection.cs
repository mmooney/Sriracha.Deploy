using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenFileStorage_Collection : IFileStorage
	{
		private readonly IDocumentSession _documentSession;

		public RavenFileStorage_Collection(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
		}

		public string StoreFile(byte[] fileData)
		{
			var item = new FileStorageDto
			{
				Id = Guid.NewGuid().ToString(),
				FileData = fileData
			};
			_documentSession.Store(item);
			_documentSession.SaveChanges();
			return item.Id;
		}


		public void UpdateFile(string fileStorageId, byte[] fileData)
		{
			var item = _documentSession.Load<FileStorageDto>(fileStorageId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(FileStorageDto), "Id", fileStorageId);
			}
			item.FileData = fileData;
			_documentSession.SaveChanges();
		}
	}
}
