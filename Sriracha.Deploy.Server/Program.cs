using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Ninject;
using NLog;

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

		private static IKernel _kernel { get; set; }
		private static Logger _logger { get; set; }

		static void Main(string[] args)
		{
			_kernel = new StandardKernel(new NinjectModules.SrirachaNinjectorator());
			SetupColorConsoleLogging();
			_logger = _kernel.Get<NLog.Logger>();

			var options = new CommandLineOptions();
			if(!Parser.Default.ParseArguments(args, options))
			{
				throw new Exception(options.GetUsage());
			}

			if (options.Debug)
			{
				Console.WriteLine("\t-Starting in debug mode...");
				try 
				{
					var service = _kernel.Get<WinService>();
					service.DebugStart();
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
				var service = _kernel.Get<WinService>();
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
