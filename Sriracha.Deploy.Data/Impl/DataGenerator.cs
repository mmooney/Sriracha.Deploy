using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class DataGenerator : IDataGenerator
	{
		private IProjectManager _projectManager;

		public DataGenerator(IProjectManager projectManager)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

		public void CreateSampleData()
		{
			var sampleProject1 = _projectManager.CreateProject("Sample Project 1", false);
			var devBranch1 = _projectManager.CreateBranch(sampleProject1.Id, "DEV Branch");
			var prodBranch1 = _projectManager.CreateBranch(sampleProject1.Id, "PROD Branch");
			var componentWeb1 = _projectManager.CreateComponent(sampleProject1.Id, "Website", false, null, EnumDeploymentIsolationType.NoIsolation);
			//_projectManager.CreateDeploymentStep(sampleProject1.Id, component1_1.Id, "Run Command Line", 
			//	var sampleProject2 = _projectManager.CreateProject("Sample Project 2");
		}
	}
}
