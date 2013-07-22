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
			if(!Directory.Exists(Path.GetDirectoryName(fileName)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));
			}
			if (append)
			{
				File.AppendAllText(fileName, text);
			}
			else 
			{
				File.WriteAllText(fileName, text);
			}
		}


		public void WriteBytes(string fileName, byte[] data)
		{
			if(!Directory.Exists(Path.GetDirectoryName(fileName)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));
			}
			File.WriteAllBytes(fileName, data);
		}
	}
}
