using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IRegexResolver
	{
		void ResolveValues(object dataObject);
	}
}
