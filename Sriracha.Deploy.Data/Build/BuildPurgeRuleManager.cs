using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.SystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Build
{
    public class BuildPurgeRuleManager : IBuildPurgeRuleManager
    {
        private readonly IBuildPurgeRuleRepository _buildPurgeRuleRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ISystemSettings _systemSettings;
        private readonly IDIFactory _diFactory;

        private List<BuildPurgeRule> _defaultBuildPurgeRuleList = new List<BuildPurgeRule>()
		{
			new BuildPurgeRule 
			{ 
                Id = "BuildPurgeRule_DefaultDEV",
				BuildRetentionMinutes = 60*24*7,
				EnvironmentNameList = new List<string> { "DEV" }
			},
			new BuildPurgeRule 
			{
                Id = "BuildPurgeRule_DefaultQAINT",
				BuildRetentionMinutes = 60*24*30,
				EnvironmentNameList = new List<string> { "QA", "INT" }
			},
			new BuildPurgeRule 
			{
                Id = "BuildPurgeRule_DefaultPROD",
				BuildRetentionMinutes = null,
				EnvironmentNameList = new List<string> { "PROD" }
			}
		};

        public BuildPurgeRuleManager(IBuildPurgeRuleRepository buildPurgeRuleRepository, IProjectRepository projectRepository, ISystemSettings systemSettings, IDIFactory diFactory)
        {
            _buildPurgeRuleRepository = DIHelper.VerifyParameter(buildPurgeRuleRepository);
            _projectRepository = DIHelper.VerifyParameter(projectRepository);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
            _diFactory = DIHelper.VerifyParameter(diFactory);
        }

        public List<BuildPurgeRule> GetSystemBuildPurgeRuleList()
        {
            var list = _buildPurgeRuleRepository.GetSystemBuildPurgeRuleList();
            if(list == null || list.Count == 0)
            {
                foreach(var rule in _defaultBuildPurgeRuleList)
                {
                    _buildPurgeRuleRepository.CreateSystemBuildPurgeRule(rule.BuildRetentionMinutes, rule.EnvironmentNameList, rule.EnvironmentIdList, rule.MachineNameList, rule.MachineIdList);
                }
                return _buildPurgeRuleRepository.GetSystemBuildPurgeRuleList();
            }
            return list;
        }

        public BuildPurgeRule CreateSystemBuildPurgeRule(int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            return _buildPurgeRuleRepository.CreateSystemBuildPurgeRule(buildRetentionMinutes, environmentNameList, environmentIdList, machineNameList, machineIdList);
        }

        public BuildPurgeRule GetSystemBuildPurgeRule(string id)
        {
            return _buildPurgeRuleRepository.GetSystemBuildPurgeRule(id);
        }

        public BuildPurgeRule UpdateSystemBuildPurgeRule(string id, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            return _buildPurgeRuleRepository.UpdateSystemBuildPurgeRule(id, buildRetentionMinutes, environmentNameList, environmentIdList, machineNameList, machineIdList);
        }

        public BuildPurgeRule DeleteSystemBuildPurgeRule(string id)
        {
            return _buildPurgeRuleRepository.DeleteSystemBuildPurgeRule(id);
        }


        public List<BuildPurgeRule> GetProjectBuildPurgeRuleList(string projectId)
        {
            return _buildPurgeRuleRepository.GetProjectBuildPurgeRuleList(projectId);
        }

        public BuildPurgeRule CreateProjectBuildPurgeRule(string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            return _buildPurgeRuleRepository.CreateProjectBuildPurgeRule(projectId, buildRetentionMinutes, environmentNameList, environmentIdList, machineNameList, machineIdList);
        }

        public BuildPurgeRule GetProjectBuildPurgeRule(string id, string projectId)
        {
            return _buildPurgeRuleRepository.GetProjectBuildPurgeRule(id, projectId);
        }

        public BuildPurgeRule UpdateProjectBuildPurgeRule(string id, string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            return _buildPurgeRuleRepository.UpdateProjectBuildPurgeRule(id, projectId, buildRetentionMinutes, environmentNameList, environmentIdList, machineNameList, machineIdList);
        }

        public int? CalculateRentionMinutes(DeployBuild build)
        {
            var systemRules = _buildPurgeRuleRepository.GetSystemBuildPurgeRuleList();
            var projectRules = _buildPurgeRuleRepository.GetProjectBuildPurgeRuleList(build.ProjectId);
            int defaultRetentionMinutes = _systemSettings.DefaultBuildRetentionMinutes;
            var ruleList = systemRules.Union(projectRules);

            var matchingRules = (from i in ruleList
                                        where i.MatchesRule(build, _diFactory)
                                        select i).ToList();
            if(matchingRules.Any(i=>!i.BuildRetentionMinutes.HasValue))
            {
                //keep forever
                return null;
            }
            else 
            {
                return matchingRules.Max(i=>i.BuildRetentionMinutes).GetValueOrDefault(defaultRetentionMinutes);
            }
        }

        public BuildPurgeRule DeleteProjectBuildPurgeRule(string id, string projectId)
        {
            return _buildPurgeRuleRepository.DeleteProjectBuildPurgeRule(id, projectId);
        }
    }
}
