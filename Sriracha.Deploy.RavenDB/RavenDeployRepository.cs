using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MMDB.Shared;
using NLog;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenDeployRepository : IDeployRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly Logger _logger;

		public RavenDeployRepository(IDocumentSession documentSession, Logger logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList)
		{
			var deployState = new DeployState
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = environment.ProjectId,
				Build = build,
				Branch = branch,
				Environment = environment,
				Component = component,
				MachineList = machineList.ToList(),
				Status = EnumDeployStatus.NotStarted,
				SubmittedDateTimeUtc = DateTime.UtcNow
			};
			_documentSession.Store(deployState);
			_documentSession.SaveChanges();
			return deployState;
		}

		public DeployState GetDeployState(string deployStateId)
		{
			if (string.IsNullOrWhiteSpace(deployStateId))
			{
				throw new ArgumentNullException("Missing Deploy State ID");
			}
			var returnValue = _documentSession.Load<DeployState>(deployStateId);
			if (returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployState), "Id", deployStateId);
			}
			return returnValue;
		}


		public DeployState PopNextDeployment()
		{
			using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				bool done = false;
				this._logger.Trace("Checking for next deployment");
				var tempItem = this._documentSession.Query<DeployState>()
										.Customize(i=>i.WaitForNonStaleResultsAsOfLastWrite(TimeSpan.FromSeconds(30)))
										.OrderBy(i=>i.SubmittedDateTimeUtc)
										.Where(i=>i.Status == EnumDeployStatus.NotStarted)
										.FirstOrDefault();
				if(tempItem == null)
				{
					this._logger.Trace("No pending deployment found");
					return null;
				}
				
				var reloadedItem = this._documentSession.Load<DeployState>(tempItem.Id);
				if(reloadedItem.Status != EnumDeployStatus.NotStarted)
				{
					this._logger.Warn("Stale pending deployment found, actual status: " + reloadedItem.Status.ToString());
					return null;
				}

				reloadedItem.Status = EnumDeployStatus.InProcess;
				this._documentSession.SaveChanges();

				transaction.Complete();

				return reloadedItem;
			}
		}
	}
}
