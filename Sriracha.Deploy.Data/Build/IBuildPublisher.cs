using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Build
{
	public interface IBuildPublisher
	{
		void PublishDirectory(BuildPublishOptions options);
		void PublishFile(BuildPublishOptions options);
		void PublishFilePattern(BuildPublishOptions options);
	}
}
