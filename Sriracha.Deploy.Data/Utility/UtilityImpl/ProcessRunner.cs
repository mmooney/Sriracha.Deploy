using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Sriracha.Deploy.Data.Credentials.CredentialsImpl;

namespace Sriracha.Deploy.Data.Utility.UtilityImpl
{
	public class ProcessRunner : IProcessRunner
	{
		//http://stackoverflow.com/questions/1520119/replicate-functionality-of-dbgview-in-net-global-win32-debug-hooks
		struct SECURITY_ATTRIBUTES
		{
			public uint Length;
			public IntPtr SecurityDescriptor;
			public bool InheritHandle;
		}
		[StructLayoutAttribute(LayoutKind.Sequential)]
		struct SECURITY_DESCRIPTOR
		{
			public byte revision;
			public byte size;
			public short control; // public SECURITY_DESCRIPTOR_CONTROL control;
			public IntPtr owner;
			public IntPtr group;
			public IntPtr sacl;
			public IntPtr dacl;
		}
		public enum SECURITY_IMPERSONATION_LEVEL
		{
			SecurityAnonymous,
			SecurityIdentification,
			SecurityImpersonation,
			SecurityDelegation
		}

		public enum TOKEN_TYPE
		{
			TokenPrimary = 1,
			TokenImpersonation
		}

		struct STARTUPINFO
		{
			public uint cb;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Reserved;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Desktop;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Title;
			public uint X;
			public uint Y;
			public uint XSize;
			public uint YSize;
			public uint XCountChars;
			public uint YCountChars;
			public uint FillAttribute;
			public uint Flags;
			public ushort ShowWindow;
			public ushort Reserverd2;
			public byte bReserverd2;
			public IntPtr StdInput;
			public IntPtr StdOutput;
			public IntPtr StdError;
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		static extern bool InitializeSecurityDescriptor(IntPtr pSecurityDescriptor, uint dwRevision);
		const uint SECURITY_DESCRIPTOR_REVISION = 1;

		[DllImport("advapi32.dll", SetLastError = true)]
		static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESCRIPTOR sd, bool daclPresent, IntPtr dacl, bool daclDefaulted);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		extern static bool DuplicateTokenEx(
			IntPtr hExistingToken,
			uint dwDesiredAccess,
			ref SECURITY_ATTRIBUTES lpTokenAttributes,
			SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
			TOKEN_TYPE TokenType,
			out IntPtr phNewToken);

		[StructLayout(LayoutKind.Sequential)]
		struct PROCESS_INFORMATION
		{
			public IntPtr Process;
			public IntPtr Thread;
			public uint ProcessId;
			public uint ThreadId;
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern bool CreateProcessAsUser(
			IntPtr Token,
			[MarshalAs(UnmanagedType.LPTStr)] string ApplicationName,
			[MarshalAs(UnmanagedType.LPTStr)] string CommandLine,
			ref SECURITY_ATTRIBUTES ProcessAttributes,
			ref SECURITY_ATTRIBUTES ThreadAttributes,
			bool InheritHandles,
			uint CreationFlags,
			IntPtr Environment,
			[MarshalAs(UnmanagedType.LPTStr)] string CurrentDirectory,
			ref STARTUPINFO StartupInfo,
			out PROCESS_INFORMATION ProcessInformation);

		[DllImport("Kernel32.dll")]
		extern static int CloseHandle(IntPtr handle);

		const UInt32 INFINITE = 0xFFFFFFFF;
		const UInt32 WAIT_ABANDONED = 0x00000080;
		const UInt32 WAIT_OBJECT_0 = 0x00000000;
		const UInt32 WAIT_TIMEOUT = 0x00000102;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);
		
		public int RunAsToken(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter, SafeHandle impersonationToken)
		{
			string logFileRoot = Guid.NewGuid().ToString();
			string stdoutLogFile = Path.Combine(Environment.CurrentDirectory, logFileRoot + ".stdout.log");
			string stderrLogFile = Path.Combine(Environment.CurrentDirectory, logFileRoot + ".stderr.log");
			string currentDirectory = Environment.CurrentDirectory;
			var primaryToken = new IntPtr();
			try
			{
				#region security attributes
				SECURITY_ATTRIBUTES processAttributes = new SECURITY_ATTRIBUTES();

				SECURITY_DESCRIPTOR sd = new SECURITY_DESCRIPTOR();
				IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(sd));
				Marshal.StructureToPtr(sd, ptr, false);
				InitializeSecurityDescriptor(ptr, SECURITY_DESCRIPTOR_REVISION);
				sd = (SECURITY_DESCRIPTOR)Marshal.PtrToStructure(ptr, typeof(SECURITY_DESCRIPTOR));

				bool result = SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false);
				if (!result)
				{
					throw new Win32Exception();
				}

				result = DuplicateTokenEx(impersonationToken.DangerousGetHandle(), 0, ref processAttributes, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out primaryToken);
				if (!result)
				{
					throw new Win32Exception();
				}
				processAttributes.SecurityDescriptor = ptr;
				processAttributes.Length = (uint)Marshal.SizeOf(sd);
				processAttributes.InheritHandle = true;
				#endregion

				SECURITY_ATTRIBUTES threadAttributes = new SECURITY_ATTRIBUTES();
				threadAttributes.SecurityDescriptor = IntPtr.Zero;
				threadAttributes.Length = 0;
				threadAttributes.InheritHandle = false;

				bool inheritHandles = true;
				//CreationFlags creationFlags = CreationFlags.CREATE_DEFAULT_ERROR_MODE;
				IntPtr environment = IntPtr.Zero;

				STARTUPINFO startupInfo = new STARTUPINFO();
				startupInfo.Desktop = "";

				PROCESS_INFORMATION processInformation;

				string cmdPath = Environment.GetEnvironmentVariable("COMSPEC");
				string cmdArguments = string.Format("/c {0} {1} > \"{2}\" 2> \"{3}\"", executablePath, executableParameters, stdoutLogFile, stderrLogFile);
				result = CreateProcessAsUser(primaryToken, cmdPath, cmdArguments, ref processAttributes, ref threadAttributes, inheritHandles, 16, environment, currentDirectory, ref startupInfo, out processInformation);
				if (!result)
				{
					long win32Code = Marshal.GetHRForLastWin32Error();
					if (win32Code == 0x80070522 )
					{
						//Log on as a service
						//Act as part of the operating system
						//Adjust memory quotas for a process
						//Replace a process level token

						//Impersonate a client after authentication?
						//Create token object?
						throw new Exception("privilges not set", new Win32Exception());
					}
					throw new Win32Exception();
				}
				WaitForSingleObject(processInformation.Process, INFINITE);

				if (File.Exists(stdoutLogFile))
				{
					standardOutputWriter.Write(File.ReadAllText(stdoutLogFile));
				}
				if (File.Exists(stderrLogFile))
				{
					errorOutputWriter.Write(File.ReadAllText(stderrLogFile));
				}

				uint exitCode;
				result = GetExitCodeProcess(processInformation.Process, out exitCode);
				if(!result)
				{
					throw new Win32Exception();
				}
				return (int)exitCode;
			}
			finally
			{
				if (primaryToken != IntPtr.Zero)
				{
					var result = CloseHandle(primaryToken);
					if (result == 0)
						throw new Win32Exception();
				}
			}
		}
		
		public int RunAsUser(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter, string domain, string userName, SecureString password)
		{
			string logFileRoot = Guid.NewGuid().ToString();
			string stdoutLogFile = Path.Combine(Environment.CurrentDirectory, logFileRoot + ".stdout.log");
			string stderrLogFile = Path.Combine(Environment.CurrentDirectory, logFileRoot + ".stderr.log");
			try 
			{
				string cmdPath = Environment.GetEnvironmentVariable("COMSPEC");
				string cmdArguments = string.Format("/c {0} {1} > \"{2}\" 2> \"{3}\"", executablePath, executableParameters, stdoutLogFile, stderrLogFile);
				var psi = new ProcessStartInfo(cmdPath, cmdArguments)
				{
					WorkingDirectory = Environment.CurrentDirectory,
					UseShellExecute = false,
					LoadUserProfile = true,
					Domain = domain,
					UserName = userName,
					Password = password
				};
				var p = new Process
				{
					StartInfo = psi
				};
				p.Start();
				p.WaitForExit();

				if (File.Exists(stdoutLogFile))
				{
					standardOutputWriter.Write(File.ReadAllText(stdoutLogFile));
				}
				if (File.Exists(stderrLogFile))
				{
					errorOutputWriter.Write(File.ReadAllText(stderrLogFile));
				}
				return p.ExitCode;
			}
			finally
			{
				//try 
				//{
				//	if(File.Exists(stdoutLogFile))
				//	{
				//		File.Delete(stdoutLogFile);
				//	}
				//}
				//catch{}
				//try 
				//{
				//	if(File.Exists(stderrLogFile))
				//	{
				//		File.Delete(stderrLogFile);
				//	}
				//}
				//catch {}
			}
		}
		public int Run(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter)
		{
			var psi = new ProcessStartInfo(executablePath, executableParameters)
			{
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			var p = new Process()
			{
				StartInfo = psi
			};
			p.OutputDataReceived += (sender, args) =>
			{
				standardOutputWriter.WriteLine(args.Data);
			};
			p.ErrorDataReceived += (sender, args) =>
			{
				errorOutputWriter.WriteLine(args.Data);
			};
			p.Start();
			p.BeginOutputReadLine();
			p.BeginErrorReadLine();
			//var p = Process.Start(psi);
			//string line;
			//while ((line = p.StandardOutput.ReadLine()) != null)
			//{
			//	standardOutputWriter.WriteLine(line);
			//}
			//while ((line = p.StandardError.ReadLine()) != null)
			//{
			//	errorOutputWriter.WriteLine(line);
			//}
			p.WaitForExit();
			return p.ExitCode;
		}
	}
}
