using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Autofac;
using CommandLine;
using CommandLine.Text;
using MMDB.Shared;
//using Ninject;
using NLog;
using Sriracha.Deploy.AutofacModules;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.Server
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		public class CommandLineOptions
		{
			[Option('d', "debug")]
			public bool Debug { get; set; }

			[Option("workingDirectory")]
			public string WorkingDirectory { get; set; }

			[Option("thrashmode")]
			public bool ThrashMode { get; set; }

            [Option("patchdata")]
            public bool PatchData { get; set; }

			[ParserState]
			public IParserState LastParserState { get; set; }

			[HelpOption]
			public string GetUsage()
			{
				return HelpText.AutoBuild(this,
					(HelpText current) =>
						HelpText.DefaultParsingErrorsHandler(this, current));
			}
		}

		//private static IKernel _kernel { get; set; }
		private static IDIFactory _diFactory;
		private static Logger _logger { get; set; }

		static void Main(string[] args)
		{
			switch(AppConfigOptions.DIContainer)
			{
				case DIContainer.Ninject:
					{
						throw new NotSupportedException("Ninject is no longer supported");
						//var kernel = new StandardKernel(new NinjectModules.SrirachaNinjectorator());
						//_logger = kernel.Get<NLog.Logger>();
						//_diFactory = kernel.Get<IDIFactory>();
					}
					//break;
				case DIContainer.Autofac:
					{
						var builder = new ContainerBuilder();
						builder.RegisterModule(new SrirachaAutofacorator(EnumDIMode.Service));
						builder.RegisterType<WinService>().AsSelf();
						var container = builder.Build();
						_logger = container.Resolve<NLog.Logger>();
						_diFactory = container.Resolve<IDIFactory>();
					}
					break;
				default:
					throw new UnknownEnumValueException(AppConfigOptions.DIContainer);
			}
			SetupColorConsoleLogging();

			var options = new CommandLineOptions();
			if(!Parser.Default.ParseArguments(args, options))
			{
				throw new Exception(options.GetUsage());
			}

			if(!string.IsNullOrWhiteSpace(options.WorkingDirectory))
			{
				var settings = _diFactory.CreateInjectedObject<ISystemSettings>();
				settings.DeployWorkingDirectory = options.WorkingDirectory;
			}
            if (options.Debug)
			{
				Console.WriteLine("\t-Starting in debug mode...");
				try 
				{
					var service = _diFactory.CreateInjectedObject<WinService>();
					service.DebugStart(options.ThrashMode);
				}
				catch(Exception err)
				{
					_logger.ErrorException("Debug mode failed: " + err.ToString(), err);
				}
				finally
				{
					Console.WriteLine("Press any key to exit");
					Console.ReadKey();
				}
			}

			else 
			{
				var service = _diFactory.CreateInjectedObject<WinService>();
				ServiceBase.Run(service);
			}
		}

		private static void SetupColorConsoleLogging()
		{
			var loggingConfig = NLog.LogManager.Configuration;
			if (loggingConfig == null)
			{
				loggingConfig = new NLog.Config.LoggingConfiguration();
			}
			var consoleTarget = new NLog.Targets.ColoredConsoleTarget();
			consoleTarget.Layout = "${longdate}:${message} ${exception:format=message,stacktrace=\r\n}";
			loggingConfig.AddTarget("consoleTarget", consoleTarget);
			var rule = new NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, consoleTarget);
			loggingConfig.LoggingRules.Add(rule);
			NLog.LogManager.Configuration = loggingConfig;
		}
	}
}
