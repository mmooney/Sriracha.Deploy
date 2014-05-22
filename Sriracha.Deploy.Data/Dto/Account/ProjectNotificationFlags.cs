using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
	public class ProjectNotificationFlags
	{
		public bool BuildPublished { get; set; }
		public bool DeployRequested {  get; set; }
		public bool DeployApproved { get; set; }
		public bool DeployRejected {  get; set; }
		public bool DeployStarted { get; set; }
		public bool DeploySuccess { get; set; }
		public bool DeployFailure { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(!(obj is ProjectNotificationFlags))
            {
                return false;
            }
            var that = (ProjectNotificationFlags)obj;
            return (this.BuildPublished == that.BuildPublished
                    && this.DeployApproved == that.DeployApproved
                    && this.DeployFailure == that.DeployFailure
                    && this.DeployRejected == that.DeployRejected
                    && this.DeployRequested == that.DeployRequested
                    && this.DeployStarted == that.DeployStarted
                    && this.DeploySuccess == that.DeploySuccess);
        }
	}
}
