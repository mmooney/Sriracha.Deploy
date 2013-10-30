using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials
{
	public interface ICredentialsManager
	{
		DeployCredentialsMasked GetMaskedCredentials(string credentialsId);
		PagedSortedList<DeployCredentialsMasked> GetMaskedCredentialList(ListOptions listOptions);

		DeployCredentials  GetCredentials(string credentialsId);
		DeployCredentialsMasked CreateCredentials(string domain, string userName, string password);
		DeployCredentialsMasked UpdateCredentials(string domain, string credentialsId, string userName, string password);

		string DecryptPassword(DeployCredentials credentials);
	}
}
