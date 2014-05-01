using roundhouse.infrastructure.logging;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Tasks.RoundhousE.DeployDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.RoundhousE
{
    public class RoundhouseLogger<TaskDefinition, TaskOptions> : Logger where TaskDefinition : IDeployTaskDefinition
    {
        private readonly TaskExecutionContext<TaskDefinition, TaskOptions> _context;

        public RoundhouseLogger(TaskExecutionContext<TaskDefinition, TaskOptions> context)
        {
            _context = context;
        }

        public void log_a_debug_event_containing(string message, params object[] args)
        {
            _context.Debug(message, args);
        }

        public void log_a_fatal_event_containing(string message, params object[] args)
        {
            _context.Error(message, args);
        }

        public void log_a_warning_event_containing(string message, params object[] args)
        {
            _context.Warn(message, args);
        }

        public void log_an_error_event_containing(string message, params object[] args)
        {
            _context.Error(message, args);
        }

        public void log_an_info_event_containing(string message, params object[] args)
        {
            _context.Info(message, args);
        }

        public object underlying_type
        {
            get { return _context; }
        }
    }
}
