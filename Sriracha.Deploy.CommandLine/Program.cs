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
using MMDB.Shared;

namespace Sriracha.Deploy.CommandLine
{
	public enum ActionType 
	{
		Unknown,
		Something,
		Deploy,
		Configure,
		Publish
	}
	public class CommandLineOptions
	{
		[CommandLineParser.Option('a',"action", Required=true)]
		public ActionType Action { get; set; }

		[CommandLineParser.Option('e', "environment")]
		public string EnvironmentId { get; set; }

		[CommandLineParser.Option('c', "component")]
		public string ComponentId { get; set; }

		[CommandLineParser.Option('m', "machine")]
		public string MachineId { get; set; }

		[CommandLineParser.Option('b', "build")]
		public string BuildId { get; set; }

		[CommandLineParser.Option("configName")]
		public string ConfigName { get; set; }
		
		[CommandLineParser.Option("configValue")]
		public string ConfigValue { get; set; }

		[CommandLineParser.Option('u',"apiurl")]
		public string ApiUrl { get; set; }

		[CommandLineParser.Option('d',"directory")]
		public string Directory { get; set; }

		[CommandLineParser.Option('f',"file")]
		public string File { get; set; }

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
						if(string.IsNullOrWhiteSpace(options.EnvironmentId))
						{
							throw new Exception("EnvironmentID (--environment|-e) required for Deploy");
						}
						if(string.IsNullOrWhiteSpace(options.BuildId))
						{
							throw new Exception("BuildID (--build|-b) required for Deploy");
						}
						Deploy(options.EnvironmentId, options.BuildId);
						break;
					case ActionType.Configure:
						if(!string.IsNullOrWhiteSpace(options.EnvironmentId) && !string.IsNullOrWhiteSpace(options.MachineId))
						{
							throw new Exception("EnvironmentID (--environment|-e) and Machine ID (--machine|-m) cannot be used at the same time for Configure");
						}
						if(string.IsNullOrWhiteSpace(options.ConfigName))
						{
							throw new Exception("ConfigName (--configName) required for Configure");
						}
						//if(string.IsNullOrWhiteSpace(options.ConfigValue))
						//{
						//	throw new Exception("ConfigValue (--configValue) required for Configure");
						//}
						if(!string.IsNullOrWhiteSpace(options.EnvironmentId) && !string.IsNullOrWhiteSpace(options.ComponentId))
						{
							ConfigureEnvironment(options.EnvironmentId, options.ComponentId, options.ConfigName, options.ConfigValue);
						}
						else if (!string.IsNullOrWhiteSpace(options.MachineId))
						{
							ConfigureMachine(options.MachineId, options.ConfigName, options.ConfigValue);
						}
						else 
						{
							throw new Exception("EnvironmentID (--environment|-e) or combination of Machine ID (--machine|-m) and Component ID (--component|-c) required for Configure");
						}
						break;
					case ActionType.Publish:
						if(string.IsNullOrWhiteSpace(options.ApiUrl))
						{
							throw new Exception("ApiUrl (--apiurl|-u) is required for Publish");
						}
						if(!string.IsNullOrWhiteSpace(options.File))
						{
							if (!string.IsNullOrWhiteSpace(options.Directory))
							{
								throw new Exception("File (--file|-f) Directory (--directory|-f) cannot be both be used together for Publish");
							}
							PublishFile(options.File, options.ApiUrl);
						}
						else if (!string.IsNullOrWhiteSpace(options.Directory))
						{
							PublishDirectory(options.Directory, options.ApiUrl);
						}
						else 
						{
							throw new Exception("Either File (--file|-f) Directory (--directory|-f) required for Publish");
						}
						break;
					default:
						throw new UnknownEnumValueException(options.Action);
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

		private static void PublishFile(string filePath, string apiUrl)
		{
			throw new NotImplementedException();
		}

		private static void PublishDirectory(string directoryPath, string apiUrl)
		{
			var publisher = Program.Kernel.Get<IBuildPublisher>();
			publisher.PublishDirectory(directoryPath, apiUrl);
		}

		private static void ConfigureMachine(string machineId, string configName, string configValue)
		{
			var pm = Program.Kernel.Get<IProjectManager>();
			pm.UpdateMachineConfig(machineId, configName, configValue);
		}

		private static void ConfigureEnvironment(string environmentId, string componentId, string configName, string configValue)
		{
			var pm = Program.Kernel.Get<IProjectManager>();
			pm.UpdateEnvironmentComponentConfig(environmentId, componentId, configName, configValue);
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
