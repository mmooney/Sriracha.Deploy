﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
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
	public class Impersonatator : IDisposable
	{
        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;
 
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

		public Impersonatator(string domain, string username, string password)
        {
            var ok = LogonUser(username, domain, password,
                           LOGON32_LOGON_NEW_CREDENTIALS, 0, out this._handle);
            if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }
 
            this._context = WindowsIdentity.Impersonate(this._handle.DangerousGetHandle());
        }
 
        public void Dispose()
        {
            this._context.Dispose();
            this._handle.Dispose();
        }
 
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);
 
        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle()
                : base(true) { }
 
            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);
 
            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }	}
}
