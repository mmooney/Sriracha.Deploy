using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
    public interface IDeployTaskStatusNotifier
    {
        event EventHandler<EventArgs<DeployState>> NotificationReceived;

        void Notify(DeployState state);
    }
}
