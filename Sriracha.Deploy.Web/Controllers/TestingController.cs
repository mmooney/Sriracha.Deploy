using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.Web.Controllers
{
    public class TestingController : Controller
    {
        private readonly IDataGenerator _dataGenerator;

		public TestingController(IDataGenerator dataGenerator)
		{
			_dataGenerator = DIHelper.VerifyParameter(dataGenerator);
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
    }
}
