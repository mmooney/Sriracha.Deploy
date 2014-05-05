using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.SystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
    public class BuildPurgeRuleService : Service
    {
        private readonly IPermissionValidator _permissionValidator;
        private readonly IBuildPurgeRuleManager _buildPurgeRuleManager;

        public BuildPurgeRuleService(IPermissionValidator permissionValidator, IBuildPurgeRuleManager buildPurgeRuleManager)
        {
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
            _buildPurgeRuleManager = DIHelper.VerifyParameter(buildPurgeRuleManager);
        }

        public object Get(BuildPurgeRuleRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("Missing request");
            }
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditBuildPurgeRules);
            if(string.IsNullOrEmpty(request.Id))
            {
                if(string.IsNullOrEmpty(request.ProjectId))
                {
                    return _buildPurgeRuleManager.GetSystemBuildPurgeRuleList();
                }
                else 
                {
                    return _buildPurgeRuleManager.GetProjectBuildPurgeRuleList(request.ProjectId);
                }
            }
            else 
            {
                if(string.IsNullOrEmpty(request.ProjectId))
                {
                    return _buildPurgeRuleManager.GetSystemBuildPurgeRule(request.Id);
                }
                else 
                {
                    return _buildPurgeRuleManager.GetProjectBuildPurgeRule(request.Id, request.ProjectId);
                }
            }
        }

        public object Put(BuildPurgeRuleRequest request)
        {
            return this.Save(request);
        }

        public object Post(BuildPurgeRuleRequest request)
        {
            return this.Save(request);
        }

        private BuildPurgeRule Save(BuildPurgeRuleRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("Request is null");
            }
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditBuildPurgeRules);
            if(string.IsNullOrEmpty(request.Id))
            {
                if(string.IsNullOrEmpty(request.ProjectId))
                {
                    return _buildPurgeRuleManager.CreateSystemBuildPurgeRule(request.BuildRetentionMinutes, request.EnvironmentNameList, request.EnvironmentIdList, request.MachineNameList, request.MachineIdList);
                }
                else 
                {
                    return _buildPurgeRuleManager.CreateProjectBuildPurgeRule(request.ProjectId, request.BuildRetentionMinutes, request.EnvironmentNameList, request.EnvironmentIdList, request.MachineNameList, request.MachineIdList);
                }
            }
            if (string.IsNullOrEmpty(request.ProjectId))
            {
                return _buildPurgeRuleManager.UpdateSystemBuildPurgeRule(request.Id, request.BuildRetentionMinutes, request.EnvironmentNameList, request.EnvironmentIdList, request.MachineNameList, request.MachineIdList);
            }
            else
            {
                return _buildPurgeRuleManager.UpdateProjectBuildPurgeRule(request.Id, request.ProjectId, request.BuildRetentionMinutes, request.EnvironmentNameList, request.EnvironmentIdList, request.MachineNameList, request.MachineIdList);
            }
        }

        public object Delete(BuildPurgeRuleRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException("request.id is null");
            }
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditBuildPurgeRules);
            if(string.IsNullOrEmpty(request.ProjectId))
            {
                return _buildPurgeRuleManager.DeleteSystemBuildPurgeRule(request.Id);
            }
            else 
            {
                return _buildPurgeRuleManager.DeleteProjectBuildPurgeRule(request.Id, request.ProjectId);
            }
        }
    }
}