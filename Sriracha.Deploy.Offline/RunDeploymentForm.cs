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
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Help_and_inconclusive_32xSM_color;
                        break;
                    case EnumDeployStatus.InProcess:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Play_32xSM_color;
                        break;
                    case EnumDeployStatus.Error:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Critical_32xSM_color;
                        break;
                    case EnumDeployStatus.Success:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Complete_and_ok_32xSM_color;
                        break;
                    default:
                        this.StatusIconImage = Properties.Resources.StatusAnnotations_Alert_32xSM_color;
                        break;
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

        private string _offlineDeploymentRunId;
        private DeployBatchRequest _batchRequest;
        private List<OfflineComponentSelection> _selectionList;
        private List<GridItem> _gridItemList;
        private string _workingDirectory;
        private readonly IDIFactory _diFactory;
        private readonly BackgroundWorker _runDeploymentWorker = new BackgroundWorker();

        public RunDeploymentForm(IDIFactory diFactory, DeployBatchRequest batchRequest, List<OfflineComponentSelection> selectionList, string workingDirectory)
        {
            InitializeComponent();
            _batchRequest = batchRequest;
            _selectionList = selectionList;
            _workingDirectory = workingDirectory;
            _diFactory = diFactory;
        }

        public RunDeploymentForm(IDIFactory diFactory, OfflineDeploymentRun offlineDeploymentRun, string workingDirectory)
        {
            InitializeComponent();
            _offlineDeploymentRunId = offlineDeploymentRun.Id;
            _batchRequest = offlineDeploymentRun.DeployBatchRequest;
            _selectionList = offlineDeploymentRun.SelectionList;
            _workingDirectory = workingDirectory;
            _diFactory = diFactory;
        }

        private void RunDeploymentForm_Load(object sender, EventArgs e)
        {
            _runDeploymentWorker.DoWork += _runDeploymentWorker_DoWork;
            _runDeploymentWorker.WorkerReportsProgress = true;
            _runDeploymentWorker.ProgressChanged += _runDeploymentWorker_ProgressChanged;
            _runDeploymentWorker.WorkerSupportsCancellation = true;
            _runDeploymentWorker.RunWorkerCompleted += _runDeploymentWorker_RunWorkerCompleted;

            UpdateBatchDetails();

            var dataProvider = _diFactory.CreateInjectedObject<IOfflineDataProvider>();
            dataProvider.Initialize(_offlineDeploymentRunId, _batchRequest, _selectionList, _workingDirectory);
            _gridItemList = new List<GridItem>();
            foreach (var component in _selectionList)
            {
                foreach (var machine in component.SelectedMachineList)
                {
                    var item = new GridItem
                    {
                        BuildDisplayValue = component.BatchRequestItem.Build.DisplayValue,
                        MachineName = machine.MachineName,
                        DeployBatchRequestItemId = component.BatchRequestItem.Id
                    };

                    var deployState = dataProvider.TryGetDeployState(component.BatchRequestItem.Build.ProjectId, component.BatchRequestItem.Build.Id, machine.EnvironmentId, machine.Id, component.BatchRequestItem.Id);
                    if(deployState == null)
                    {
                        item.SetStatus(EnumDeployStatus.NotStarted);
                    }
                    else 
                    {
                        item.Update(deployState);
                    }
                    _gridItemList.Add(item);
                }
            }
            this._grdStatus.AutoGenerateColumns = false;
            _grdStatus.DataSource = _gridItemList;
        }


        void _runDeploymentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _btnStart.Enabled = true;
            _btnCancel.Enabled = false;
        }

        void _runDeploymentWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e.UserState is DeployState)
            {
                var state = (DeployState)e.UserState;
                var item = _gridItemList.FirstOrDefault(i=>i.DeployBatchRequestItemId == state.DeployBatchRequestItemId);
                if(item != null)
                {
                    item.Update(state);
                    _grdStatus.Refresh();
                }
            }
            else if(e.UserState is DeployBatchRequest)
            {
                _batchRequest = (DeployBatchRequest)e.UserState;
                this.UpdateBatchDetails();
            }
        }

        void _runDeploymentWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var notifier = _diFactory.CreateInjectedObject<IDeployStatusNotifier>();
            notifier.DeployStateNotificationReceived += notifier_DeployStateNotificationReceived;
            notifier.BatchRequestNotificationReceived += notifier_BatchRequestNotificationReceived;

            IDeployBatchRunner batchRunner = _diFactory.CreateInjectedObject<IDeployBatchRunner>();
            batchRunner.ForceRunDeployment(_batchRequest.Id);
        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            var dataProvider = _diFactory.CreateInjectedObject<IOfflineDataProvider>();
            dataProvider.Initialize(_offlineDeploymentRunId, _batchRequest, _selectionList, _workingDirectory);

            var systemSettings = _diFactory.CreateInjectedObject<ISystemSettings>();
            systemSettings.DeployWorkingDirectory = _workingDirectory;

            _btnStart.Enabled = false;
            _btnCancel.Enabled = true;
            _runDeploymentWorker.RunWorkerAsync(_batchRequest.Id);
        }

        void notifier_DeployStateNotificationReceived(object sender, EventArgs<DeployState> e)
        {
            _runDeploymentWorker.ReportProgress(0, e.Value);
        }

        void notifier_BatchRequestNotificationReceived(object sender, EventArgs<DeployBatchRequest> e)
        {
            _runDeploymentWorker.ReportProgress(0, e.Value);
        }


        private void _btnCancel_Click(object sender, EventArgs e)
        {
            var deployRequestManager = _diFactory.CreateInjectedObject<IDeployRequestManager>();
            deployRequestManager.PerformAction(_batchRequest.Id, EnumDeployBatchAction.Cancel, null);
        }

        private void _grdStatus_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == _colViewDetails.Index)
            {
                var item = _gridItemList[e.RowIndex];
                var deployBatchRequestItem = _batchRequest.ItemList.Single(i=>i.Id == item.DeployBatchRequestItemId);
                using(var dlg = new ViewDeployStateForm(deployBatchRequestItem, item.DeployState))
                {
                    dlg.ShowDialog();
                }
            }
        }

        private void UpdateBatchDetails()
        {
            _lblLabel.Text = _batchRequest.DeploymentLabel;
            _lblStatus.Text = _batchRequest.StatusDisplayValue;
            if (_batchRequest.StartedDateTimeUtc.HasValue)
            {
                _lblStarted.Text = WinFormsHelper.LocalDateText(_batchRequest.StartedDateTimeUtc.Value);
            }
            else
            {
                _lblStarted.Text = "N/A";
            }
            if (_batchRequest.CompleteDateTimeUtc.HasValue)
            {
                _lblCompleted.Text = WinFormsHelper.LocalDateText(_batchRequest.CompleteDateTimeUtc.Value);
            }
            else
            {
                _lblCompleted.Text = "N/A";
            }
        }

        private void RunDeploymentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var notifier = _diFactory.CreateInjectedObject<IDeployStatusNotifier>();
            notifier.DeployStateNotificationReceived -= notifier_DeployStateNotificationReceived;
            notifier.BatchRequestNotificationReceived -= notifier_BatchRequestNotificationReceived;
        }
    }
}
