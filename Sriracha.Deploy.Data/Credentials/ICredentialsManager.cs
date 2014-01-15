using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace Sriracha.Deploy.Data.Credentials
{
	public interface ICredentialsManager
	{
		DeployCredentialsMasked GetMaskedCredentials(string credentialsId);
		PagedSortedList<DeployCredentialsMasked> GetMaskedCredentialList(ListOptions listOptions);

		DeployCredentials  GetCredentials(string credentialsId);
		DeployCredentialsMasked CreateCredentials(string domain, string userName, string password);
		DeployCredentialsMasked UpdateCredentials(string credentialsId, string domain, string userName, string password);
        DeployCredentialsMasked DeleteCredentials(string credentialsId);

		string DecryptPassword(DeployCredentials credentials);
		SecureString DecryptPasswordSecure(DeployCredentials credentials);

    }
}
