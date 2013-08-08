using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.CommandLine
{
	public enum DIContainer
	{
		Ninject,
		Autofac
	}
	public static class AppConfigOptions
	{
		public static DIContainer DIContainer
		{
			get { return DIContainer.Autofac; }
		}
	}
}
