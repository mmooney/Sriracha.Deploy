using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class FileWriter : IFileWriter
	{
		public void WriteText(string fileName, string text, bool append)
		{
			if(append)
			{
				File.AppendAllText(fileName, text);
			}
			else 
			{
				File.WriteAllText(fileName, text);
			}
		}
	}
}
