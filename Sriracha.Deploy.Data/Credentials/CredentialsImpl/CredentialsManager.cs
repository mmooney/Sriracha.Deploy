using PagedList;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Credentials;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials.CredentialsImpl
{
	public class CredentialsManager : ICredentialsManager
	{
		private readonly ICredentialsRepository _credentialsRepository;
		private readonly IEncrypterator _encrypterator;

		static CredentialsManager()
		{
			AutoMapper.Mapper.CreateMap<DeployCredentials, DeployCredentialsMasked>();
		}

		public CredentialsManager(ICredentialsRepository credentialsRepository, IEncrypterator encrypterator)
		{
			_credentialsRepository = DIHelper.VerifyParameter(credentialsRepository);
			_encrypterator = DIHelper.VerifyParameter(encrypterator);
		}


		public DeployCredentialsMasked GetMaskedCredentials(string credentialsId)
		{
			var item = _credentialsRepository.GetCredentials(credentialsId);
			return AutoMapper.Mapper.Map(item, new DeployCredentialsMasked());
		}

		public PagedSortedList<DeployCredentialsMasked> GetMaskedCredentialList(ListOptions listOptions)
		{
			var list = _credentialsRepository.GetCredentialsList(listOptions);
			var mappedList = list.Items.Select(i=>AutoMapper.Mapper.Map(i, new DeployCredentialsMasked())).ToList();
			var pagedList = new StaticPagedList<DeployCredentialsMasked>(mappedList, list.PageNumber, list.PageSize, list.TotalItemCount);
			return new PagedSortedList<DeployCredentialsMasked>(pagedList, list.SortField, list.SortAscending);
		}

		public DeployCredentialsMasked CreateCredentials(string domain, string userName, string password)
		{
			var encrytpedPassword = EncryptPassword(userName, password);
			var item = _credentialsRepository.CreateCredentials(domain, userName, encrytpedPassword);
			return AutoMapper.Mapper.Map(item, new DeployCredentialsMasked());
		}

		public DeployCredentialsMasked UpdateCredentials(string credentialsId, string domain, string userName, string password)
		{
			var encrytpedPassword = EncryptPassword(userName, password);
			var item = _credentialsRepository.UpdateCredentials(credentialsId, domain, userName, encrytpedPassword);
			return AutoMapper.Mapper.Map(item, new DeployCredentialsMasked());
		}

		private string EncryptPassword(string userName, string password)
		{
			return _encrypterator.Encrypt(userName, password);
		}

		private string DecrypedPassword(string userName, string encryptedPassword)
		{
			return _encrypterator.Decrypt(userName, encryptedPassword);
		}


		public DeployCredentials GetCredentials(string credentialsId)
		{
			return _credentialsRepository.GetCredentials(credentialsId);
		}

		public string DecryptPassword(DeployCredentials credentials)
		{
			return this.DecrypedPassword(credentials.UserName, credentials.EncryptedPassword);
		}
	}
}
