using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.TaskImpl
{
    public class DeployTaskStatusNotifier : IDeployTaskStatusNotifier
    {
        public event EventHandler<EventArgs<DeployState>> NotificationReceived;

        public void Notify(DeployState state)
        {
            if (this.NotificationReceived != null)
            {
                var args = new EventArgs<DeployState>(state);
                this.NotificationReceived(this, args);
            }
        }
    }
}
