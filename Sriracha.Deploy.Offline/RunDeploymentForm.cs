using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Deployment.DeploymentImpl;
using Sriracha.Deploy.Data.Deployment.Offline;
using Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Project;
using Sriracha.Deploy.Data.Project.ProjectImpl;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sriracha.Deploy.Offline
{
    public partial class RunDeploymentForm : Form
    {
        private class GridItem
        {
            public void SetStatus(EnumDeployStatus status)
            {
                switch(status)
                {
                    case EnumDeployStatus.NotStarted:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Help_and_inconclusive_32xMD_color;
                        break;
                    case EnumDeployStatus.InProcess:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Play_32xMD_color;
                        break;
                    case EnumDeployStatus.Error:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Critical_32xMD_color;
                        break;
                    default:
                        throw new UnknownEnumValueException(status);
                }
            }
            public Image StatusIconImage { get; set; }
            public string BuildDisplayValue { get; set; }
            public string MachineName { get; set; }
        }

        private DeployBatchRequest _batchRequest;
        private List<OfflineComponentSelection> _selectionList;
        private List<GridItem> _gridItemList;
        private string _workingDirectory;
        private readonly IDIFactory _diFactory;

        public RunDeploymentForm(IDIFactory diFactory, DeployBatchRequest batchRequest, List<OfflineComponentSelection> selectionList, string workingDirectory)
        {
            InitializeComponent();
            _batchRequest = batchRequest;
            _selectionList = selectionList;
            _workingDirectory = workingDirectory;
            _diFactory = diFactory;

            _gridItemList = new List<GridItem>();
            foreach(var component in selectionList)
            {
                foreach(var machine in component.SelectedMachineList)
                {
                    var item = new GridItem
                    {
                        BuildDisplayValue = component.BatchRequestItem.Build.DisplayValue,
                        MachineName = machine.MachineName
                    };
                    item.SetStatus(EnumDeployStatus.NotStarted);
                    _gridItemList.Add(item);
                }
            }
            _grdStatus.DataSource = _gridItemList;
        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            var dataProvider = _diFactory.CreateInjectedObject<IOfflineDataProvider>();
            dataProvider.Initialize(_batchRequest, _selectionList, _workingDirectory);

            var systemSettings = _diFactory.CreateInjectedObject<ISystemSettings>();
            systemSettings.DeployWorkingDirectory = _workingDirectory;

            IDeployBatchRunner batchRunner = _diFactory.CreateInjectedObject<IDeployBatchRunner>();
            batchRunner.ForceRunDeployment(_batchRequest.Id);
        }
    }
}
