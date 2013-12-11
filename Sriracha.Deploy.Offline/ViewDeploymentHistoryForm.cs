using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment.Offline;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Offline.Properties;
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
    public partial class ViewDeploymentHistoryForm : Form
    {
        private class GridItem
        {
            public Image StatusIconImage { get; set; }
            public string StatusDisplayValue { get; set; }
            public string StartedDateDisplayValue { get; set; }
            public string CompletedDateDisplayValue { get; set; }
            public string OfflineDeploymentRunId { get; set; }

            public GridItem(OfflineDeploymentRun data)
            {
                this.OfflineDeploymentRunId = data.Id;
                this.StatusDisplayValue = EnumHelper.GetDisplayValue(data.DeployBatchRequest.Status);
                switch(data.DeployBatchRequest.Status)
                {
                    case EnumDeployStatus.Cancelled:
                        this.StatusIconImage =  Resources.StatusAnnotations_Stop_32xSM_color;
                        break;
                    case EnumDeployStatus.Error:
                        this.StatusIconImage = Resources.StatusAnnotations_Critical_32xSM_color;
                        break;
                    case EnumDeployStatus.InProcess:
                        this.StatusIconImage = Resources.StatusAnnotations_Play_32xSM_color;
                        break;
                    case EnumDeployStatus.NotStarted:
                        this.StatusIconImage = Resources.StatusAnnotations_Help_and_inconclusive_32xSM_color;
                        break;
                    case EnumDeployStatus.Success:
                        this.StatusIconImage = Resources.StatusAnnotations_Complete_and_ok_32xSM_color;
                        break;
                    default:
                        this.StatusIconImage = Resources.StatusAnnotations_Alert_32xSM_color;
                        break;
                }
                if(data.DeployBatchRequest.StartedDateTimeUtc.HasValue)
                {
                    this.StartedDateDisplayValue = WinFormsHelper.LocalDateText(data.DeployBatchRequest.StartedDateTimeUtc.Value);
                }
                if(data.DeployBatchRequest.CompleteDateTimeUtc.HasValue)
                {
                    this.StartedDateDisplayValue = WinFormsHelper.LocalDateText(data.DeployBatchRequest.CompleteDateTimeUtc.Value);
                }
            }
        }

        private DeployBatchRequest _batchRequest;
        private string _workingDirectory;
        private readonly IDIFactory _diFactory;
        private List<GridItem> _gridItemList;
        private List<OfflineDeploymentRun> _deployHistoryList;

        public ViewDeploymentHistoryForm(IDIFactory diFactory, DeployBatchRequest batchRequest, string workingDirectory)
        {
            _batchRequest = batchRequest;
            _workingDirectory = workingDirectory;
            _diFactory = diFactory;
            InitializeComponent();
            _grdHistory.AutoGenerateColumns = false;
        }

        private void ViewDeploymentHistoryForm_Load(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void LoadData()
        {
            _lblLabel.Text = _batchRequest.DeploymentLabel;
            _lblDirectory.Text = _workingDirectory;

            var dataProvider = _diFactory.CreateInjectedObject<IOfflineDataProvider>();
            _deployHistoryList = dataProvider.GetDeployHistoryList(_workingDirectory);
            _gridItemList = new List<GridItem>();
            foreach (var historyItem in _deployHistoryList)
            {
                var gridItem = new GridItem(historyItem);
                _gridItemList.Add(gridItem);
            }
            _grdHistory.DataSource = _gridItemList;
        }

        private void _grdHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == _colViewRerun.Index)
            {
                var gridItem= _gridItemList[e.RowIndex];
                var offlineDeploymentRun = _deployHistoryList.Single(i => i.Id == gridItem.OfflineDeploymentRunId);
                using(var dlg = new RunDeploymentForm(_diFactory, offlineDeploymentRun, _workingDirectory))
                {
                    dlg.ShowDialog();
                }
                this.LoadData();
            }
        }

        private void _btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
