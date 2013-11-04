using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository
{
	public abstract class RepositoryTestBase<T>
	{
		protected abstract T GetRepository();
	}
}
