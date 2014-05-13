using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
    public class RavenBuildPurgeRuleRepository : IBuildPurgeRuleRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly IUserIdentity _userIdentity;

        public RavenBuildPurgeRuleRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public List<BuildPurgeRule> GetSystemBuildPurgeRuleList()
        {
            return _documentSession.QueryNoCache<BuildPurgeRule>().Where(i=>i.ProjectId == null).ToList();
        }

        public BuildPurgeRule GetSystemBuildPurgeRule(string id)
        {
            return _documentSession.LoadEnsureNoCache<BuildPurgeRule>(id);
        }

        public BuildPurgeRule CreateSystemBuildPurgeRule(int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            var item = new BuildPurgeRule
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = null,
                BuildRetentionMinutes = buildRetentionMinutes,
                EnvironmentIdList = new List<string>(environmentIdList),
                EnvironmentNameList = new List<string>(environmentNameList),
                MachineIdList = new List<string>(machineIdList),
                MachineNameList = new List<string>(machineNameList),
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow, 
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            return _documentSession.StoreSaveEvict(item);
        }

        public BuildPurgeRule UpdateSystemBuildPurgeRule(string id, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            var item = _documentSession.LoadEnsure<BuildPurgeRule>(id);
            item.BuildRetentionMinutes = buildRetentionMinutes;
            item.ProjectId = null;
            item.EnvironmentNameList = new List<string>(environmentNameList);
            item.EnvironmentIdList = new List<string>(environmentIdList);
            item.MachineNameList = new List<string>(machineNameList);
            item.MachineIdList = new List<string>(machineIdList);
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
            item.UpdatedByUserName = _userIdentity.UserName;
            return _documentSession.SaveEvict(item);
        }

        public BuildPurgeRule DeleteSystemBuildPurgeRule(string id)
        {
            var item = _documentSession.LoadEnsure<BuildPurgeRule>(id);
            return _documentSession.DeleteSaveEvict(item);
        }

        public List<BuildPurgeRule> GetProjectBuildPurgeRuleList(string projectId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            return _documentSession.QueryNoCache<BuildPurgeRule>().Where(i => i.ProjectId == projectId).ToList();
        }

        public BuildPurgeRule GetProjectBuildPurgeRule(string id, string projectId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Missing ID");
            }
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            var item = _documentSession.QueryNoCache<BuildPurgeRule>().SingleOrDefault(i => i.Id == id && i.ProjectId == projectId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(BuildPurgeRule), "Id_ProjectId", id + "_" + projectId);
            }
            return item;
        }

        public BuildPurgeRule CreateProjectBuildPurgeRule(string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            var item = new BuildPurgeRule
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                BuildRetentionMinutes = buildRetentionMinutes,
                EnvironmentIdList = new List<string>(environmentIdList),
                EnvironmentNameList = new List<string>(environmentNameList),
                MachineIdList = new List<string>(machineIdList),
                MachineNameList = new List<string>(machineNameList),
                CreatedDateTimeUtc = DateTime.UtcNow,
                CreatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName 
            };
            return _documentSession.StoreSaveEvict(item);
        }

        public BuildPurgeRule UpdateProjectBuildPurgeRule(string id, string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Missing ID");
            }
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            var item = _documentSession.Query<BuildPurgeRule>().SingleOrDefault(i => i.Id == id && i.ProjectId == projectId);
            if (item == null)
            {
                throw new RecordNotFoundException(typeof(BuildPurgeRule), "Id_ProjectId", id + "_" + projectId);
            }
            item.BuildRetentionMinutes = buildRetentionMinutes;
            item.ProjectId = projectId;
            item.EnvironmentNameList = new List<string>(environmentNameList);
            item.EnvironmentIdList = new List<string>(environmentIdList);
            item.MachineNameList = new List<string>(machineNameList);
            item.MachineIdList = new List<string>(machineIdList);
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
            item.UpdatedByUserName = _userIdentity.UserName;
            return _documentSession.SaveEvict(item);
        }


        public BuildPurgeRule DeleteProjectBuildPurgeRule(string id, string projectId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Missing ID");
            }
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            var item = _documentSession.Query<BuildPurgeRule>().SingleOrDefault(i => i.Id == id && i.ProjectId == projectId);
            if (item == null)
            {
                throw new RecordNotFoundException(typeof(BuildPurgeRule), "Id_ProjectId", id + "_" + projectId);
            }
            return _documentSession.DeleteSaveEvict(item);
        }
    }
}
