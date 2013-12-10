using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using System.IO;
using Sriracha.Deploy.Data.Utility;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class OfflineDeploymentManager : IOfflineDeploymentManager
	{
		private readonly IDeployRepository _deployRepository;
		private readonly IOfflineDeploymentRepository _offlineDeploymentRepository;
        private readonly ISystemSettings _systemSettings;
        private readonly IFileRepository _fileRepository;
        private readonly IZipper _zipper;

		public OfflineDeploymentManager(IDeployRepository deployRepository, IOfflineDeploymentRepository offlineDeploymentRepository, ISystemSettings systemSettings, IFileRepository fileRepository, IZipper zipper)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
			_offlineDeploymentRepository = DIHelper.VerifyParameter(offlineDeploymentRepository);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
            _fileRepository = DIHelper.VerifyParameter(fileRepository);
            _zipper = DIHelper.VerifyParameter(zipper);
		}
		public OfflineDeployment BeginCreateOfflineDeployment(List<DeployBatchRequestItem> itemList, string deploymentLabel)
		{
			var batchRequest = _deployRepository.CreateBatchRequest(itemList, DateTime.UtcNow, EnumDeployStatus.OfflineRequested, deploymentLabel);
			return _offlineDeploymentRepository.CreateOfflineDeployment(batchRequest.Id, EnumOfflineDeploymentStatus.CreateRequested);
		}

        public OfflineDeployment GetOfflineDeployment(string offlineDeploymentId)
        {
            return _offlineDeploymentRepository.GetOfflineDeployment(offlineDeploymentId);
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
                        var fileData = _fileRepository.GetFileData(item.Build.FileId);
                        File.WriteAllBytes(Path.Combine(targetPackagePath, item.Build.Id + ".zip"), fileData);
                    }
                }
                string targetZipPath = targetDirectory + ".zip";
                _zipper.ZipDirectory(targetDirectory, targetZipPath);
                var zippedFileBytes = File.ReadAllBytes(targetZipPath);
                var fileObject = _fileRepository.CreateFile(deployBatchRequest.Id + ".zip", zippedFileBytes);
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
    }
}
