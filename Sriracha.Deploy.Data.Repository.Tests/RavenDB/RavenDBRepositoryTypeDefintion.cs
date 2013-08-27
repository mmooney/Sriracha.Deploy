using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;

namespace Sriracha.Deploy.Data.Repository.Tests.RavenDB
{
	public class RavenDBRepositoryTypeDefintion<RepositoryType> : RepositoryTypeDefinition
	{
		public RavenDBRepositoryTypeDefintion()
		{
		}

		public override object CreateRepository()
		{
			List<object> parameterList = new List<object>();
			var constructor = typeof(RepositoryType).GetConstructors().First();
			foreach(var parameterDefinition in constructor.GetParameters())
			{
				if(parameterDefinition.ParameterType.IsAssignableFrom(typeof(IDocumentSession)))
				{
					var session = EmbeddedRavenProvider.DocumentStore.OpenSession();
					parameterList.Add(session);
					_disposableList.Add(session);
				}
				else 
				{
					throw new Exception(string.Format("Cannot create \"{0}\", unrecognoized constructor parameter type \"{1}\"", typeof(RepositoryType).FullName, parameterDefinition.ParameterType.FullName));
				}
			}
			return constructor.Invoke(parameterList.ToArray());
		}
	}
}
