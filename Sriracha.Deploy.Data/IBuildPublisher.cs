using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IBuildPublisher
	{
		void PublishDirectory(string directoryPath, string apiUrl);
	}
}
