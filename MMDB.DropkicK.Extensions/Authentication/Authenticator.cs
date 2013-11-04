using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.DropkicK.Extensions.Authentication
{
    public static class Authenticator
    {
        public static FileShareAuthenticationContext BeginFileShareAuthentication(string remoteUnc, string userName, string password)
        {
            string error = PinvokeWindowsNetworking.connectToRemote(remoteUnc, userName, password);
            if(!string.IsNullOrEmpty(error))
            {
                throw new Exception("Error calling PinvokeWindowsNetworking.connectToRemote: " + error);
            }
            return new FileShareAuthenticationContext(remoteUnc, userName, password);
        }
    }
}
