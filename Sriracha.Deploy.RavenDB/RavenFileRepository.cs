using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenFileRepository : IFileRepository
	{
		public Data.Dto.DeployFile StoreFile(string fileName, byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}
