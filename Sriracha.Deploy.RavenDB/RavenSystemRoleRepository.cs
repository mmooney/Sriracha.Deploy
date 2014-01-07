using NLog;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
    public class RavenSystemRoleRepository : ISystemRoleRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly IUserIdentity _userIdentity;

        public RavenSystemRoleRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            return _documentSession.QueryNoCache<SystemRole>().Where(i => i.Assignments.UserNameList.Any(j => j == userName)).ToList();
        }

        public SystemRole TryGetSystemEveryoneRole()
        {
            return _documentSession.QueryNoCache<SystemRole>().FirstOrDefault(i=>i.EveryoneRoleIndicator);
        }
    }
}
