using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sriracha.Deploy.Data.Repository.Tests
{
	public abstract class RepositoryTypeDefinition : IDisposable
	{
		protected List<IDisposable> _disposableList;

		public RepositoryTypeDefinition()
		{
			_disposableList = new List<IDisposable>();
		}

		public abstract object CreateRepository();

		private void DisposeAll()
		{
			if(_disposableList != null && _disposableList.Count > 0)
			{
				var list = _disposableList.ToArray();
				foreach(var item in list)
				{
					using(item)
					{
						item.Dispose();
					}
					_disposableList.Remove(item);
				}
			}
		}

		public void Dispose()
		{
			this.DisposeAll();
		}
	}
}
