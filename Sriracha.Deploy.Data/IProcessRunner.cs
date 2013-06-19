using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IProcessRunner
	{
		void Run(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter);
	}
}
