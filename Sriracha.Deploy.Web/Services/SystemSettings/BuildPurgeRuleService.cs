using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
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
        private readonly ISystemSettings _systemSettings;

        public BuildPurgeRuleService(IPermissionValidator permissionValidator, ISystemSettings systemSettings)
        {
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
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
                return _systemSettings.BuildPurgeRetentionRuleList;
            }
            else 
            {
                var item = _systemSettings.BuildPurgeRetentionRuleList.FirstOrDefault(i=>i.Id == request.Id);
                if(item == null)
                {
                    throw new ArgumentException("Invalid ID " + request.Id);
                }
                return item;
            }
        }
    }
}