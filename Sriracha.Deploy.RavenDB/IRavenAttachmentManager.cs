using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
	public interface IRavenAttachmentManager
	{
		byte[] GetAttachment(string attachmentId);
		string GetAttachmentString(string attachmentId);
		void SetAttachment(string attachmentId, byte[] fileData);
		void SetAttachment(string attachmentId, string attachmentData);
		void SetAttachment(string attachmentId, Stream stream);
		void RemoveAttachment(string attachmentId);
	}
}
