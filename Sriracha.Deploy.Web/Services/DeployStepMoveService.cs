using MMDB.Shared;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services
{
    public class DeployStepMoveService : Service
    {
        private readonly IPermissionValidator _permissionValidator;
        private readonly IProjectManager _projectManager;

        public DeployStepMoveService(IPermissionValidator permissionValidator, IProjectManager projectManager)
        {
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
            _projectManager = DIHelper.VerifyParameter(projectManager);
        }

        public object Post(DeployStepMoveRequest request)
        {
            return this.Move(request);
        }

        public object Put(DeployStepMoveRequest request)
        {
            return this.Move(request);
        }

        private DeployStep Move(DeployStepMoveRequest request)
        {
            //_permissionValidator.VerifyCurrentUserProjectPermission(request.ProjectId, EnumPermissionAccess.
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException("request.id is null");
            }
            if (string.IsNullOrEmpty(request.ProjectId))
            {
                throw new ArgumentNullException("request.projectId is null");
            }
            if (string.IsNullOrEmpty(request.ParentId))
            {
                throw new ArgumentNullException("request.parentId is null");
            }
            if (request.ParentType == EnumDeployStepParentType.Unknown)
            {
                throw new ArgumentNullException("request.parentType is null");
            }
            switch(request.Direction)
            {
                case DeployStepMoveRequest.EnumDirection.Up:
                    return _projectManager.MoveDeploymentStepUp(request.ProjectId, request.ParentType, request.ParentId, request.Id);
                case DeployStepMoveRequest.EnumDirection.Down:
                    return _projectManager.MoveDeploymentStepDown(request.ProjectId, request.ParentType, request.ParentId, request.Id);
                default:
                    throw new UnknownEnumValueException(request.Direction);
            }
        }
    }
}