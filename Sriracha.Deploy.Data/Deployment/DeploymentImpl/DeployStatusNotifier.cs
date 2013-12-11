using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
    public class DeployStatusNotifier : IDeployStatusNotifier
    {
        public event EventHandler<EventArgs<DeployState>> DeployStateNotificationReceived;
        public event EventHandler<EventArgs<DeployBatchRequest>> BatchRequestNotificationReceived;

        public void Notify(DeployState state)
        {
            if (this.DeployStateNotificationReceived != null)
            {
                var args = new EventArgs<DeployState>(state);
                this.DeployStateNotificationReceived(this, args);
            }
        }


        public void Notify(DeployBatchRequest batchRequest)
        {
            if(this.BatchRequestNotificationReceived != null)
            {
                var args = new EventArgs<DeployBatchRequest>(batchRequest);
                this.BatchRequestNotificationReceived(this, args);
            }
        }
    }
}
