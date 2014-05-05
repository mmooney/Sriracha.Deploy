using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Build
{
    public interface IBuildPurgeRuleManager
    {
        List<BuildPurgeRule> GetSystemBuildPurgeRuleList();
        BuildPurgeRule CreateSystemBuildPurgeRule(int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList);
        BuildPurgeRule GetSystemBuildPurgeRule(string id);
        BuildPurgeRule UpdateSystemBuildPurgeRule(string id, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList);
        BuildPurgeRule DeleteSystemBuildPurgeRule(string id);

        List<BuildPurgeRule> GetProjectBuildPurgeRuleList(string projectId);
        BuildPurgeRule CreateProjectBuildPurgeRule(string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList);
        BuildPurgeRule GetProjectBuildPurgeRule(string id, string projectId);
        BuildPurgeRule UpdateProjectBuildPurgeRule(string id, string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList);
        BuildPurgeRule DeleteProjectBuildPurgeRule(string id, string projectId);

        int? CalculateRentionMinutes(DeployBuild build);
    }
}
