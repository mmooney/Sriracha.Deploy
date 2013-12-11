using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Deployment;
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
    public partial class ViewDeployStateForm : Form
    {
        private DeployBatchRequestItem _deployBatchRequestItem;
        private DeployState _deployState;

        public ViewDeployStateForm(DeployBatchRequestItem deployBatchRequestItem, DeployState deployState)
        {
            _deployBatchRequestItem = deployBatchRequestItem;
            _deployState = deployState;
            InitializeComponent();
        }

        private void ViewDeployStateForm_Load(object sender, EventArgs e)
        {
            _lblProjectComponentBranch.Text = string.Format("{0} - {1} - {2}", _deployBatchRequestItem.Build.ProjectName, _deployBatchRequestItem.Build.ProjectComponentName, _deployBatchRequestItem.Build.ProjectBranchName);
            _lblEnvironmentMachine.Text = string.Format("{0} - {1}", _deployBatchRequestItem.MachineList.First().EnvironmentName, _deployBatchRequestItem.MachineList.First().MachineName);
            _lblBuild.Text = _deployBatchRequestItem.Build.DisplayValue;
            _lblStartedDate.Text =  _lblCompletedDate.Text = "N/A";
            _lblStatus.Text = EnumHelper.GetDisplayValue(EnumDeployStatus.NotStarted);
            if(_deployState != null)
            {
                if(_deployState.DeploymentStartedDateTimeUtc.HasValue)
                {
                    _lblStartedDate.Text = WinFormsHelper.LocalDateText(_deployState.DeploymentStartedDateTimeUtc.Value);
                }
                if(_deployState.DeploymentCompleteDateTimeUtc.HasValue)
                {
                    _lblCompletedDate.Text = WinFormsHelper.LocalDateText(_deployState.DeploymentCompleteDateTimeUtc.Value);
                }
                _lblStatus.Text = _deployState.StatusDisplayValue;

                var messageList = _deployState.MessageList.OrderByDescending(i=>i.DateTimeUtc);
                foreach(var message in messageList)
                {
                    string displayValue = string.Format("{0} - {1}", WinFormsHelper.LocalDateText(message.DateTimeUtc), message.Message);
                    _txtMessageList.AppendText(displayValue + Environment.NewLine);
                    _txtMessageList.AppendText("---------------------------" + Environment.NewLine);
                }
            }
        }
    }
}
