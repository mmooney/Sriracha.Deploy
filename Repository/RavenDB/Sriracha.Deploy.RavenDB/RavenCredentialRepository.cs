using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Credentials;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenCredentialRepository : ICredentialsRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;

		public RavenCredentialRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		private string FormatId(string domain, string userName)
		{
			string returnValue = "DeployCredentials_" + userName.Replace("\\", "_");
			if(!string.IsNullOrEmpty(domain))
			{
				returnValue += "_" + domain.Replace("\\","_");
			}
			return returnValue;
		}

		public PagedSortedList<DeployCredentials> GetCredentialsList(ListOptions listOptions)
		{
			listOptions = listOptions ?? new ListOptions();
			var pagedList = _documentSession.QueryPageAndSort<DeployCredentials>(listOptions, "UserName", true, null);
			return new PagedSortedList<DeployCredentials>(pagedList, listOptions.SortField, listOptions.SortAscending.GetValueOrDefault());
		}

		public DeployCredentials GetCredentials(string credentialsId)
		{
			var item = _documentSession.LoadEnsureNoCache<DeployCredentials>(credentialsId);
			if(item == null) 
			{
				throw new RecordNotFoundException(typeof(DeployCredentials), "Id", credentialsId);
			}
			return item;
		}


		public DeployCredentials CreateCredentials(string domain, string userName, string encrytpedPassword)
		{
			string id = FormatId(userName, domain);
			var existingItem = _documentSession.LoadNoCache<DeployCredentials>(id);
			if(existingItem != null)
			{
				throw new ArgumentException("UserName already exists: " + userName);
			}
			var item = new DeployCredentials
			{
				Id = id,
				Domain = domain,
				UserName = userName,
				EncryptedPassword = encrytpedPassword,
				CreatedByUserName = _userIdentity.UserName,
				CreatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow
			};
			_documentSession.Store(item);
			_documentSession.SaveChanges();
			return item;
		}

		public DeployCredentials UpdateCredentials(string credentialsId, string domain, string userName, string encrytpedPassword)
		{
			var item = _documentSession.LoadEnsure<DeployCredentials>(credentialsId);
			item.Domain = domain;
			item.UserName = userName;
			item.EncryptedPassword = encrytpedPassword;
			item.UpdatedByUserName = _userIdentity.UserName;
			item.UpdatedDateTimeUtc = DateTime.UtcNow;
			_documentSession.SaveChanges();
			return item;
		}


        public DeployCredentials DeleteCredentials(string credentialsId)
        {
            var item = _documentSession.LoadEnsure<DeployCredentials>(credentialsId);
            return _documentSession.DeleteSaveEvict(item);
        }
    }
}
