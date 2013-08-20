using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Web.Controllers
{
    public class TestingController : Controller
    {
        private readonly IDataGenerator _dataGenerator;
		private readonly IFileRepository _fileRepository;

		public TestingController(IDataGenerator dataGenerator, IFileRepository fileRepository)
		{
			_dataGenerator = DIHelper.VerifyParameter(dataGenerator);
			_fileRepository = DIHelper.VerifyParameter(fileRepository);
		}
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult CreateSampleData()
		{
			_dataGenerator.CreateSampleData();
			return Content("Done");
		}

		public ActionResult File(string deployFileId, string contentType="application/octet-stream")
		{
			var fileObject = _fileRepository.GetFile(deployFileId);
			var data = _fileRepository.GetFileData(deployFileId);
			return File(data, contentType, fileObject.FileName);	
		}
    }
}
