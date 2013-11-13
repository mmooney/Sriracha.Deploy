using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.DropkicK.Extensions.Authentication
{
    public class FileShareAuthenticationContext  : IDisposable
    {
        private readonly string _remoteUnc;
        private readonly string _userName;
        private readonly string _password;
        private bool _active;

        public FileShareAuthenticationContext(string remoteUnc, string userName, string password)
        {
            _remoteUnc = remoteUnc;
            _userName = userName;
            _password = password;
            _active = true;
        }

        public void Dispose()
        {
            if(_active)
            {  
                var error = PinvokeWindowsNetworking.disconnectRemote(_remoteUnc);
                if(!string.IsNullOrEmpty(error))
                {
                    throw new Exception("PinvokeWindowsNetworking.disconnectRemote failed: " + error);
                }
                _active = false;
            }
        }
    }
}
