using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class ProcessRunner : IProcessRunner
	{
		public int Run(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter)
		{
			var psi = new ProcessStartInfo(executablePath, executableParameters);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;
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
