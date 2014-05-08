using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services
{
    [Route("/project/{projectId}/{parentType}/{parentId}/step/{id}/move/{direction}")]
    public class DeployStepMoveRequest
    {
        public enum EnumDirection
        {
            Unknown,
            Up,
            Down
        }
        public string ProjectId { get; set; }
        public EnumDeployStepParentType ParentType { get; set; }
        public string ParentId { get; set; }
        public string Id { get; set; }
        public EnumDirection Direction { get; set; }

    }
}