using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public enum EnumEmailMessageStatus
	{
		New,
		InProcess,
		Failed,
		Success
	}

	public class SrirachaEmailMessage
	{
		public class SrirachaEmailMessageRecipientResult
		{
			public string Id { get; set; }
			public string SrirachaEmailMessageId { get; set; }
			public EnumEmailMessageStatus Status { get; set; }
			public string EmailAddress { get; set; }
			public string Details { get; set; }
			public DateTime StatusDateTimeUtc { get; set; }
		}

		public string Id { get; set; }
		public string Subject { get; set; }
		public object DataObject { get; set; }
		public string RazorView { get; set; }
		public EnumEmailMessageStatus Status { get; set; }
		public DateTime QueueDateTimeUtc { get; set; }
		public List<string> EmailAddressList { get; set; }
		public List<SrirachaEmailMessageRecipientResult> RecipientResultList { get; set; }

		public SrirachaEmailMessage()
		{
			this.EmailAddressList = new List<string>();
			this.RecipientResultList = new List<SrirachaEmailMessageRecipientResult>();
		}

	}
}
