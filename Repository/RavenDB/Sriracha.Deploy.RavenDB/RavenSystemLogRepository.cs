using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using NLog;
using PagedList;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenSystemLogRepository : ISystemLogRepository
	{
        //public class SystemLogByDateIndex : AbstractIndexCreationTask<SystemLog>
        //{
        //    public SystemLogByDateIndex()
        //    {
        //        Map = messages => from i in messages
        //                          select new { i.MessageDateTimeUtc, i.EnumSystemLogTypeID };
        //        Index(x => x.MessageDateTimeUtc, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        //    }
        //}

		private readonly IDocumentSession _documentSession;
		private readonly Logger _logger;

		public RavenSystemLogRepository(IDocumentSession documentSession, Logger logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public SystemLog LogMessage(EnumSystemLogType logType, string userName, DateTime messageDateTime, string message, string loggerName)
		{
			var systemLog = new SystemLog
			{
				Id = Guid.NewGuid().ToString(),
				EnumSystemLogTypeID = logType,
				UserName = userName,
				MessageDateTimeUtc = messageDateTime,
				MessageText = message,
				LoggerName = loggerName
			};
			this._documentSession.StoreSaveEvict(systemLog);
			return systemLog;
		}


        public SystemLog GetMessage(string id)
        {
            return _documentSession.LoadEnsureNoCache<SystemLog>(id);
        }

        public PagedSortedList<SystemLog> GetList(ListOptions listOptions, List<EnumSystemLogType> systemLogTypeList = null)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "MessageDateTimeUtc", false);
            var query = _documentSession.QueryNoCache<SystemLog>();
            if(systemLogTypeList != null && systemLogTypeList.Any())
            {
                query = query.Where(i=>i.EnumSystemLogTypeID.In(systemLogTypeList));
            }
            switch (listOptions.SortField)
            {
                case "MessageDateTimeUtc":
                    return query.PageAndSort(listOptions, i => i.MessageDateTimeUtc);
                default:
                    throw new ArgumentException("Unrecognized Sort Field " + listOptions.SortField);
            }
        }

        public void PurgeLogMessages(DateTime utcNow, EnumSystemLogType systemLogType, int? ageMinutes)
		{
            //if (ageMinutes.HasValue)
            //{
            //    DateTime maxDate = utcNow.AddMinutes(0 - ageMinutes.Value);
            //    this._logger.Trace(string.Format("Deleting {0} messages older than {1}", systemLogType, maxDate));
            //    var query = this._documentSession.Advanced.LuceneQuery<SystemLog, SystemLogByDateIndex>()
            //                                    .WhereEquals("EnumSystemLogTypeID", systemLogType)
            //                                    .AndAlso().WhereLessThan("MessageDateTimeUtc", maxDate);
            //    var queryString = query.ToString();
            //    this._documentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex
            //    (
            //        typeof(SystemLogByDateIndex).Name, new IndexQuery
            //        {
            //            Query = queryString
            //        },
            //        allowStale: true
            //    );
            //    this._documentSession.SaveChanges();
            //}

		}

    }
}
