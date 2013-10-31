using Microsoft.Win32.SafeHandles;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials.CredentialsImpl
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class Impersonator : IImpersonator
	{
		private enum EnumLogonType
		{
			/// <summary>
			/// This logon type is intended for users who will be interactively using the computer, such as a user being logged on  
			/// by a terminal server, remote shell, or similar process.
			/// This logon type has the additional expense of caching logon information for disconnected operations; 
			/// therefore, it is inappropriate for some client/server applications,
			/// such as a mail server.
			/// </summary>
			LOGON32_LOGON_INTERACTIVE = 2,

			/// <summary>
			/// This logon type is intended for high performance servers to authenticate plaintext passwords.

			/// The LogonUser function does not cache credentials for this logon type.
			/// </summary>
			LOGON32_LOGON_NETWORK = 3,

			/// <summary>
			/// This logon type is intended for batch servers, where processes may be executing on behalf of a user without 
			/// their direct intervention. This type is also for higher performance servers that process many plaintext
			/// authentication attempts at a time, such as mail or Web servers. 
			/// The LogonUser function does not cache credentials for this logon type.
			/// </summary>
			LOGON32_LOGON_BATCH = 4,

			/// <summary>
			/// Indicates a service-type logon. The account provided must have the service privilege enabled. 
			/// </summary>
			LOGON32_LOGON_SERVICE = 5,

			/// <summary>
			/// This logon type is for GINA DLLs that log on users who will be interactively using the computer. 
			/// This logon type can generate a unique audit record that shows when the workstation was unlocked. 
			/// </summary>
			LOGON32_LOGON_UNLOCK = 7,

			/// <summary>
			/// This logon type preserves the name and password in the authentication package, which allows the server to make 
			/// connections to other network servers while impersonating the client. A server can accept plaintext credentials 
			/// from a client, call LogonUser, verify that the user can access the system across the network, and still 
			/// communicate with other servers.
			/// NOTE: Windows NT:  This value is not supported. 
			/// </summary>
			LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

			/// <summary>
			/// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
			/// The new logon session has the same local identifier but uses different credentials for other network connections. 
			/// NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
			/// NOTE: Windows NT:  This value is not supported. 
			/// </summary>
			LOGON32_LOGON_NEW_CREDENTIALS = 9,
		}

		private enum EnumLogonProvider
		{
			/// <summary>
			/// Use the standard logon provider for the system. 
			/// The default security provider is negotiate, unless you pass NULL for the domain name and the user name 
			/// is not in UPN format. In this case, the default provider is NTLM. 
			/// NOTE: Windows 2000/NT:   The default security provider is NTLM.
			/// </summary>
			LOGON32_PROVIDER_DEFAULT = 0,
			LOGON32_PROVIDER_WINNT35 = 1,
			LOGON32_PROVIDER_WINNT40 = 2,
			LOGON32_PROVIDER_WINNT50 = 3
		}

		private readonly ICredentialsManager _credentialsManager;
		
		public Impersonator(ICredentialsManager credentialsManager)
		{
			_credentialsManager = DIHelper.VerifyParameter(credentialsManager);
		}

		public ImpersonationContext BeginImpersonation(string credentialsId)
        {
			var credentials = _credentialsManager.GetCredentials(credentialsId);
			string password = _credentialsManager.DecryptPassword(credentials);
			//var ok = LogonUser(username, domain, password,
			//			   LOGON32_LOGON_NEW_CREDENTIALS, 0, out this._handle);
			ImpersonationContext.SafeTokenHandle handle;
			var loginType = EnumLogonType.LOGON32_LOGON_INTERACTIVE;
			//loginType = EnumLogonType.LOGON32_LOGON_BATCH;
			loginType = EnumLogonType.LOGON32_LOGON_NEW_CREDENTIALS;
			var ok = LogonUser(credentials.UserName, credentials.Domain, password, (int)loginType, (int)EnumLogonProvider.LOGON32_PROVIDER_DEFAULT, out handle);
			if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();
				string win32message = new Win32Exception(errorCode).Message;
				string errorMessage = string.Format("Could not impersonate {0}, LogonUser returned error code {1}, error message: {2}", credentials.DisplayValue, errorCode, win32message);
				throw new ApplicationException(errorMessage);
            }
            var context = WindowsIdentity.Impersonate(handle.DangerousGetHandle());
			var maskedCredentials = _credentialsManager.GetMaskedCredentials(credentialsId);
			return new ImpersonationContext(maskedCredentials, handle, context);
        }
 
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out ImpersonationContext.SafeTokenHandle phToken);
	}
}
