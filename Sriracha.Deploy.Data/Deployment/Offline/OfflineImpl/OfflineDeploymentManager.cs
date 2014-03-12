using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using System.IO;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Dto.Project;
using MMDB.Shared;
using System.Xml;
using Ionic.Zip;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.SystemSettings;
using Sriracha.Deploy.Data.Build;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
	public class OfflineDeploymentManager : IOfflineDeploymentManager
	{
		private readonly IDeployRepository _deployRepository;
        private readonly IDeployStateRepository _deployStateRepository;
		private readonly IOfflineDeploymentRepository _offlineDeploymentRepository;
        private readonly ISystemSettings _systemSettings;
        private readonly IFileRepository _fileRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IZipper _zipper;
        private readonly IManifestBuilder _manifestBuilder;

		public OfflineDeploymentManager(IDeployRepository deployRepository, IDeployStateRepository deployStateRepository, IOfflineDeploymentRepository offlineDeploymentRepository, IProjectRepository projectRepository, ISystemSettings systemSettings, IFileRepository fileRepository, IZipper zipper, IManifestBuilder manifestBuilder)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
            _deployStateRepository = DIHelper.VerifyParameter(deployStateRepository);
			_offlineDeploymentRepository = DIHelper.VerifyParameter(offlineDeploymentRepository);
            _projectRepository = DIHelper.VerifyParameter(projectRepository);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
            _fileRepository = DIHelper.VerifyParameter(fileRepository);
            _zipper = DIHelper.VerifyParameter(zipper);
            _manifestBuilder = DIHelper.VerifyParameter(manifestBuilder);
		}

		public OfflineDeployment BeginCreateOfflineDeployment(string deployBatchRequestId)
		{
            var batchRequest = _deployRepository.GetBatchRequest(deployBatchRequestId);
            batchRequest = _deployRepository.UpdateBatchDeploymentStatus(batchRequest.Id, EnumDeployStatus.OfflineRequested);
			return _offlineDeploymentRepository.CreateOfflineDeployment(batchRequest.Id, EnumOfflineDeploymentStatus.CreateRequested);
		}

        public OfflineDeployment GetOfflineDeployment(string offlineDeploymentId)
        {
            return _offlineDeploymentRepository.GetOfflineDeployment(offlineDeploymentId);
        }


        public OfflineDeployment GetOfflineDeploymentForDeploymentBatchRequestId(string deployBatchRequestId)
        {
            return _offlineDeploymentRepository.GetOfflineDeploymentForDeploymentBatchRequestId(deployBatchRequestId);
        }

        public OfflineDeployment PopNextOfflineDeploymentToCreate()
        {
            return _offlineDeploymentRepository.PopNextOfflineDeploymentToCreate();
        }

        public void CreateOfflineDeploymentPackage(string offlineDeploymentId)
        {
            var offlineDeployment = _offlineDeploymentRepository.GetOfflineDeployment(offlineDeploymentId);
            try 
            {
                string targetDirectory = Path.Combine(Path.GetTempPath(), "Offline", offlineDeploymentId.ToString(), Guid.NewGuid().ToString());
                
                if(!Directory.Exists(_systemSettings.OfflineExeDirectory))
                {
                    throw new Exception("Unable to find offline exe directory at " + _systemSettings.OfflineExeDirectory);
                }
                if(!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                foreach(var fileName in Directory.GetFiles(_systemSettings.OfflineExeDirectory))
                {
                    File.Copy(fileName, Path.Combine(targetDirectory, Path.GetFileName(fileName)));
                }
                foreach(var exeConfigFileName in Directory.GetFiles(targetDirectory, "*.exe.config"))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(exeConfigFileName);
                    bool anyUpdates = false;
                    var siteUrlAppSettingNodes = xmlDoc.SelectNodes("//appSettings/add[@key='SiteUrl']/@value");
                    if(siteUrlAppSettingNodes != null)
                    {
                        foreach(XmlNode node in siteUrlAppSettingNodes)
                        {
                            node.Value = AppSettingsHelper.GetSetting("SiteUrl");
                            anyUpdates = true;
                        }
                    }
                    if(anyUpdates)
                    {
                        xmlDoc.Save(exeConfigFileName);
                    }
                }
                var deployBatchRequest = _deployRepository.GetBatchRequest(offlineDeployment.DeployBatchRequestId);
                var requestJson = deployBatchRequest.ToJson();
                File.WriteAllText(Path.Combine(targetDirectory, "request.json"), requestJson);

                string targetPackagePath = Path.Combine(targetDirectory, "packages"); 
                Directory.CreateDirectory(targetPackagePath);
                List<string> loadedBuilds = new List<string>();
                foreach(var item in deployBatchRequest.ItemList)
                {
                    if(item.Build == null)
                    {
                        throw new ArgumentNullException(string.Format("Build property is null for item " + item.Id));
                    }
                    if(!loadedBuilds.Contains(item.Build.Id))
                    {
                        var file = _fileRepository.GetFile(item.Build.FileId);
                        var fileJson = file.ToJson();
                        File.WriteAllText(Path.Combine(targetPackagePath, file.Id + ".json"), fileJson);

                        var fileData = _fileRepository.GetFileData(file.Id);
                        File.WriteAllBytes(Path.Combine(targetPackagePath, file.Id + ".data"), fileData);
                    }
                }

                var projectIdList = deployBatchRequest.ItemList.Select(i=>i.Build.ProjectId).Distinct();
                foreach(var projectId in projectIdList)
                {
                    var dbProject = _projectRepository.GetProject(projectId);
                    var outputProject = AutoMapper.Mapper.Map(dbProject, new DeployProject());
                    var environmentsToDelete = new List<DeployEnvironment>();
                    foreach(var environment in outputProject.EnvironmentList)
                    {
                        if(!deployBatchRequest.ItemList.Any(i=>i.Build.ProjectId == outputProject.Id && i.MachineList.Any(j=>j.EnvironmentId == environment.Id)))
                        {
                            environmentsToDelete.Add(environment);
                        }
                    }
                    foreach(var environment in environmentsToDelete)
                    {   
                        outputProject.EnvironmentList.Remove(environment);
                    }

                    var targetProjectDirectory = Path.Combine(targetDirectory, "projects");
                    if(!Directory.Exists(targetProjectDirectory))
                    {
                        Directory.CreateDirectory(targetProjectDirectory);
                    }
                    string projectJson = outputProject.ToJson();
                    string projectFilePath = Path.Combine(targetProjectDirectory, outputProject.Id + ".json");
                    File.WriteAllText(projectFilePath, projectJson);
                }
                string targetZipPath = targetDirectory + ".zip";
                _zipper.ZipDirectory(targetDirectory, targetZipPath);
                var zippedFileBytes = File.ReadAllBytes(targetZipPath);
                var fileManifest = _manifestBuilder.BuildFileManifest(zippedFileBytes);
                var fileObject = _fileRepository.CreateFile(deployBatchRequest.Id + ".zip", zippedFileBytes, fileManifest);
                _offlineDeploymentRepository.SetReadyForDownload(offlineDeploymentId, fileObject.Id);

                try 
                {
                    Directory.Delete(targetDirectory, true);
                }
                catch {}
                try 
                {
                    File.Delete(targetZipPath);
                }
                catch {}
            }
            catch(Exception err)
            {
                _offlineDeploymentRepository.UpdateStatus(offlineDeploymentId, EnumOfflineDeploymentStatus.CreateFailed, err);
            }
        }

        public OfflineDeployment ImportHistory(string offlineDeploymentId, string fileId)
        {
            var offlineDeployment = _offlineDeploymentRepository.GetOfflineDeployment(offlineDeploymentId);
            var fileMetadata = _fileRepository.GetFile(fileId);

            using(var fileStream = _fileRepository.GetFileDataStream(fileId))
            using(var zipFile = ZipFile.Read(fileStream))
            {
                foreach(var entry in zipFile)
                {
                    if(!entry.IsDirectory && !entry.IsText)
                    {
                        using(var entryStream = entry.OpenReader()) 
                        using(var entryStreamReader = new StreamReader(entryStream))
                        {
                            string json = entryStreamReader.ReadToEnd();
                            var newDeployState = JsonConvert.DeserializeObject<DeployState>(json);
                            newDeployState.DeployBatchRequestItemId = offlineDeployment.DeployBatchRequestId;
                            _deployStateRepository.ImportDeployState(newDeployState);
                        }
                    }
                }
            }
            _deployRepository.UpdateBatchDeploymentStatus(offlineDeployment.DeployBatchRequestId, EnumDeployStatus.OfflineComplete);
            return offlineDeployment;
        }
    }
}
