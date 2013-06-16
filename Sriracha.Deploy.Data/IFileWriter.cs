using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IFileWriter
	{
		void WriteText(string fileName, string text, bool append);
	}
}
