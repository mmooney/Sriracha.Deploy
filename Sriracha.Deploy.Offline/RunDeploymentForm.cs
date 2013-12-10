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
using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Sriracha.Deploy.Offline
{
    public partial class RunDeploymentForm : Form
    {
        private class GridItem
        {
            public void SetStatus(EnumDeployStatus status)
            {
                this.StatusDisplayValue = EnumHelper.GetDisplayValue(status);
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
            public string StatusDisplayValue { get; set; }
            public string BuildDisplayValue { get; set; }
            public string MachineName { get; set; }
            public string DeployBatchRequestItemId { get; set; }
            public DeployState DeployState { get; set; }

            public void Update(DeployState state)
            {
                this.DeployState = state;
                this.SetStatus(state.Status);
            }
        }

        private DeployBatchRequest _batchRequest;
        private List<OfflineComponentSelection> _selectionList;
        private List<GridItem> _gridItemList;
        private string _workingDirectory;
        private readonly IDIFactory _diFactory;
        private readonly BackgroundWorker _runDeploymentWorker;

        public RunDeploymentForm(IDIFactory diFactory, DeployBatchRequest batchRequest, List<OfflineComponentSelection> selectionList, string workingDirectory)
        {
            InitializeComponent();
            _batchRequest = batchRequest;
            _selectionList = selectionList;
            _workingDirectory = workingDirectory;
            _diFactory = diFactory;

            _runDeploymentWorker = new BackgroundWorker();
            _runDeploymentWorker.DoWork += _runDeploymentWorker_DoWork;
            _runDeploymentWorker.WorkerReportsProgress = true;
            _runDeploymentWorker.ProgressChanged += _runDeploymentWorker_ProgressChanged;

            _gridItemList = new List<GridItem>();
            foreach(var component in selectionList)
            {
                foreach(var machine in component.SelectedMachineList)
                {
                    var item = new GridItem
                    {
                        BuildDisplayValue = component.BatchRequestItem.Build.DisplayValue,
                        MachineName = machine.MachineName,
                        DeployBatchRequestItemId = component.BatchRequestItem.Id
                    };
                    item.SetStatus(EnumDeployStatus.NotStarted);
                    _gridItemList.Add(item);
                }
            }
            _grdStatus.DataSource = _gridItemList;
        }

        void _runDeploymentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var state = (DeployState)e.UserState;
            var item = _gridItemList.FirstOrDefault(i=>i.DeployBatchRequestItemId == state.DeployBatchRequestItemId);
            if(item != null)
            {
                item.Update(state);
            }
        }

        void _runDeploymentWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var notifier = _diFactory.CreateInjectedObject<IDeployTaskStatusNotifier>();
            notifier.NotificationReceived += notifier_NotificationReceived;

            IDeployBatchRunner batchRunner = _diFactory.CreateInjectedObject<IDeployBatchRunner>();
            batchRunner.ForceRunDeployment(_batchRequest.Id);
        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            var dataProvider = _diFactory.CreateInjectedObject<IOfflineDataProvider>();
            //because threading ouch
            //var copiedBatchRequest = AutoMapper.Mapper.Map(_batchRequest, new DeployBatchRequest());
            //var copiedSelectionList = AutoMapper.Mapper.Map(_selectionList, new List<OfflineComponentSelection>());
            dataProvider.Initialize(_batchRequest, _selectionList, _workingDirectory);

            var systemSettings = _diFactory.CreateInjectedObject<ISystemSettings>();
            systemSettings.DeployWorkingDirectory = _workingDirectory;

            _runDeploymentWorker.RunWorkerAsync(_batchRequest.Id);
        }

        void notifier_NotificationReceived(object sender, EventArgs<DeployState> e)
        {
            _runDeploymentWorker.ReportProgress(0, e.Value);
        }
    }
}
