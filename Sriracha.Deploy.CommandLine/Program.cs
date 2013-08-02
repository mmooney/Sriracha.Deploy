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

		[CommandLineParser.Option("branch")]
		public string BranchId { get; set; }

		[CommandLineParser.Option('p', "project")]
		public string ProjectId { get; set; }

		[CommandLineParser.Option('v', "version")]
		public string Version { get; set; }

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

		[CommandLineParser.Option("pause")]
		public bool Pause { get; set; }

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
			bool pause = false;
			Program.Logger = Program.Kernel.Get<NLog.Logger>();
			try 
			{
				var options = new CommandLineOptions();
				if(!CommandLineParser.Parser.Default.ParseArguments(args, options) || options.Action == ActionType.Unknown)
				{
					throw new Exception(options.GetUsage());
				}
				pause = options.Pause;
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
						VerifyParameter(options.ApiUrl,"Publish", "ApiUrl", "apiurl", 'a');
						VerifyParameter(options.ProjectId,"Publish", "ProjectId", "project", 'p');
						VerifyParameter(options.BranchId,"Publish", "BranchId", "branch");
						VerifyParameter(options.ComponentId,"Publish", "ComponentId", "component", 'c');
						VerifyParameter(options.Version,"Publish", "Version", "version", 'v');
						if(!string.IsNullOrWhiteSpace(options.File))
						{
							if (!string.IsNullOrWhiteSpace(options.Directory))
							{
								throw new Exception("File (--file|-f) Directory (--directory|-f) cannot be both be used together for Publish");
							}
							PublishFile(options.File, options.ApiUrl, options.ProjectId, options.ComponentId, options.BranchId, options.Version);
						}
						else if (!string.IsNullOrWhiteSpace(options.Directory))
						{
							PublishDirectory(options.Directory, options.ApiUrl, options.ProjectId, options.ComponentId, options.BranchId, options.Version);
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
				Program.Logger.ErrorException("Error: " + err.ToString(), err);
			}
			if(pause)
			{
				Console.WriteLine("Please any key to continue");
				Console.ReadKey();
			}
		}

		private static void VerifyParameter(string value, string actionName, string parameterName, string longName, char? shortName=null)
		{
			if(string.IsNullOrWhiteSpace(value))
			{
				string message;
				if(shortName.HasValue)
				{
					message = string.Format("{0} (--{1}|-{2}) required for {3}", parameterName, longName, shortName, actionName);
				}
				else 
				{
					message = string.Format("{0} (--{1}) required for {2}", parameterName, longName, actionName);
				}
				throw new ArgumentException(message);
			}
		}

		private static void PublishFile(string filePath, string apiUrl, string projectId, string componentId, string branchId, string version)
		{
			var publisher = Program.Kernel.Get<IBuildPublisher>();
			publisher.PublishFile(filePath, apiUrl, projectId, componentId, branchId, version);
		}

		private static void PublishDirectory(string directoryPath, string apiUrl, string projectId, string componentId, string branchId, string version)
		{
			var publisher = Program.Kernel.Get<IBuildPublisher>();
			publisher.PublishDirectory(directoryPath, apiUrl, projectId, componentId, branchId, version);
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

			deploymentRunner.Deploy(null, environmentID, buildID, runtimeSystemSettings);

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
