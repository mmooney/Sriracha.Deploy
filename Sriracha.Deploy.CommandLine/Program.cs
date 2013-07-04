using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLineParser = CommandLine;
using Ninject;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;
using System.Reflection;
using System.IO;

namespace Sriracha.Deploy.CommandLine
{
	public enum ActionType 
	{
		Unknown,
		Something,
		Deploy
	}
	public class CommandLineOptions
	{
		[CommandLineParser.Option('a',"action", Required=true)]
		public ActionType Action { get; set; }

		[CommandLineParser.Option('e',"environment")]
		public string EnvironmentID { get; set; }

		[CommandLineParser.Option('b', "build")]
		public string BuildID { get; set; }

		[CommandLineParser.ParserState]
		public CommandLineParser.IParserState LastParserState { get; set; }

		[CommandLineParser.HelpOption]
		public string GetUsage() 
		{
			return CommandLineParser.Text.HelpText.AutoBuild(this, 
				(CommandLineParser.Text.HelpText current) => 
					CommandLineParser.Text.HelpText.DefaultParsingErrorsHandler(this, current));
		}
  	}

	public class Program
	{
		private static IKernel Kernel { get; set; }
		private static NLog.Logger Logger { get; set; }

		static void Main(string[] args)
		{
			Program.Kernel = new StandardKernel(new NinjectModules.SrirachaNinjectorator());
			SetupColorConsoleLogging();
			Program.Logger = Program.Kernel.Get<NLog.Logger>();
			try 
			{
				var options = new CommandLineOptions();
				if(!CommandLineParser.Parser.Default.ParseArguments(args, options) || options.Action == ActionType.Unknown)
				{
					throw new Exception(options.GetUsage());
				}
				switch(options.Action)
				{
					case ActionType.Deploy:
						if(string.IsNullOrWhiteSpace(options.EnvironmentID))
						{
							throw new Exception("EnvironmentID (--environment|-e) required");
						}
						if(string.IsNullOrWhiteSpace(options.BuildID))
						{
							throw new Exception("BuildID (--build|-b) required");
						}
						Deploy(options.EnvironmentID, options.BuildID);
						break;
				}
				if(args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
				{
					throw new Exception("All your sriracha are belong to us");
				}
			}
			catch(Exception err)
			{
				Program.Logger.ErrorException("Error", err);
			}
			Console.WriteLine("Please any key to continue");
			Console.ReadKey();
		}

		private static void Deploy(string environmentID, string buildID)
		{
			Program.Logger.Info("Executing deployment request for build " + buildID + " to environment " + environmentID);

			var deploymentRunner = Program.Kernel.Get<IDeployRunner>();
			var runtimeSystemSettings = new RuntimeSystemSettings
			{
				LocalDeployDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Guid.NewGuid().ToString())
			};
			Directory.CreateDirectory(runtimeSystemSettings.LocalDeployDirectory);
			Program.Logger.Info("\tUsing path: " + runtimeSystemSettings.LocalDeployDirectory);

			deploymentRunner.Deploy(environmentID, buildID, runtimeSystemSettings);

			Program.Logger.Info("Done executing deployment request for build " + buildID + " to environment " + environmentID);
		}

		private static void SetupColorConsoleLogging()
		{
			var loggingConfig = NLog.LogManager.Configuration;
			if(loggingConfig == null)
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
