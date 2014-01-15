using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineCredentialsRepository : ICredentialsRepository
    {
        public Dto.PagedSortedList<Dto.Credentials.DeployCredentials> GetCredentialsList(Dto.ListOptions listOptions)
        {
            throw new NotImplementedException();
        }

        public Dto.Credentials.DeployCredentials GetCredentials(string credentialsId)
        {
            throw new NotImplementedException();
        }

        public Dto.Credentials.DeployCredentials CreateCredentials(string domain, string userName, string encrytpedPassword)
        {
            throw new NotImplementedException();
        }

        public Dto.Credentials.DeployCredentials UpdateCredentials(string credentialsId, string domain, string userName, string encrytpedPassword)
        {
            throw new NotImplementedException();
        }


        public Dto.Credentials.DeployCredentials DeleteCredentials(string credentialsId)
        {
            throw new NotImplementedException();
        }
    }
}
