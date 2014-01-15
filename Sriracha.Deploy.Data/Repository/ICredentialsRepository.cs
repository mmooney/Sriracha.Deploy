using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
	public interface ICredentialsRepository
	{
		PagedSortedList<DeployCredentials> GetCredentialsList(ListOptions listOptions);
		DeployCredentials GetCredentials(string credentialsId);

		DeployCredentials CreateCredentials(string domain, string userName, string encrytpedPassword);
		DeployCredentials UpdateCredentials(string credentialsId, string domain, string userName, string encrytpedPassword);
        DeployCredentials DeleteCredentials(string credentialsId);
    }
}
