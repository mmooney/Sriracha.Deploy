﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Sriracha.Deploy.Data.Utility.UtilityImpl
{
	public class ProcessRunner : IProcessRunner
	{
		public int Run(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter, string domain, string userName, SecureString password)
		{
			var psi = new ProcessStartInfo(executablePath, executableParameters)
			{
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				Domain = domain,
				UserName = userName,
				Password = password,
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
			p.WaitForExit();
			return p.ExitCode;
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
