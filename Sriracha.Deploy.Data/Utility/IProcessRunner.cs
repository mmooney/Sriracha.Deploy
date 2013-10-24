using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Utility
{
	public interface IProcessRunner
	{
		int Run(string executablePath, string executableParameters, TextWriter standardOutputWriter, TextWriter errorOutputWriter);
	}
}
