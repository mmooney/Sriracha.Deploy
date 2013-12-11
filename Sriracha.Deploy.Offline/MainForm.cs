using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sriracha.Deploy.Offline
{
    public partial class MainForm : Form
    {
        private DeployBatchRequest _batchRequest;
        private readonly IDIFactory _diFactory;

        public MainForm(IDIFactory diFactory)
        {
            InitializeComponent();
            _diFactory = diFactory;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _pnlDeploymentInfo.Visible = false;
            _btnContinue.Visible = false;

            string defaultFile = Path.Combine(Environment.CurrentDirectory, "request.json");
            if(File.Exists(defaultFile))
            {
                this.LoadBatchRequestFile(defaultFile);
            }
        }

        private void _btnRequestFileNameBrowse_Click(object sender, EventArgs e)
        {
            using(var dlg = new OpenFileDialog())
            {
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.FileName = _txtRequestFileName.Text;
                dlg.Filter = "Sriracha Batch Request Files (*.json)|*.json|All Files (*.*)|*.*";
                if(!string.IsNullOrEmpty(_txtRequestFileName.Text) && File.Exists(_txtRequestFileName.Text))
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(_txtRequestFileName.Text);
                }
                else 
                {
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                }
                dlg.Title = "Open Sriracha Batch Request File";
                
                var result = dlg.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    this.LoadBatchRequestFile(dlg.FileName);
                }
            }
        }

        private void LoadBatchRequestFile(string filePath)
        {
            try 
            {
                if(!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Batch Request File Not Found", filePath);
                }
                string jsonData = File.ReadAllText(filePath);
                if(string.IsNullOrEmpty(jsonData))
                {
                    throw new ArgumentException(string.Format("File is empty \"{0}\"", filePath));
                }
                _batchRequest = JsonConvert.DeserializeObject<DeployBatchRequest>(jsonData);
                _txtRequestFileName.Text = filePath;

                _pnlAllComponents.Controls.Clear();
                foreach(var item in _batchRequest.ItemList)
                {
                    var ctrl = new ComponentSelectionControl(item);
                    _pnlAllComponents.Controls.Add(ctrl);
                }
                
                _pnlDeploymentInfo.Visible = true;
                _btnContinue.Visible = true;
            }
            catch(Exception err)
            {
                WinFormsHelper.DisplayError(string.Format("Error loading batch file name \"{0}\": {1}", filePath, err.Message), err);
            }
        }

        private void _btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach(ComponentSelectionControl ctrl in _pnlAllComponents.Controls)
            {
                ctrl.CheckAllMachines(true);
            }
        }

        private void _btnClearAll_Click(object sender, EventArgs e)
        {
            foreach (ComponentSelectionControl ctrl in _pnlAllComponents.Controls)
            {
                ctrl.CheckAllMachines(false);
            }
        }

        private void _btnSelectMachines_Click(object sender, EventArgs e)
        {
            using(var dlg = new SelectMachineForm(_batchRequest))
            {
                var result = dlg.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    var machineNameList = dlg.GetSelectedMachineNameList();
                    foreach (ComponentSelectionControl ctrl in _pnlAllComponents.Controls)
                    {
                        ctrl.CheckSpecificMachines(machineNameList);
                    }
                }
            }
        }

        private void _btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _btnContinue_Click(object sender, EventArgs e)
        {
            var selectionList = new List<OfflineComponentSelection>();
            foreach (ComponentSelectionControl ctrl in _pnlAllComponents.Controls)
            {
                selectionList.Add(ctrl.GetComponentSelection());
            }
            if(!selectionList.Any(i=>i.SelectedMachineList.Any()))
            {
                MessageBox.Show("Please select at least one component/machine to deploy");
                return;
            }
            using(var dlg = new RunDeploymentForm(_diFactory, _batchRequest, selectionList, Path.GetDirectoryName(_txtRequestFileName.Text)))
            {
                dlg.ShowDialog();
            }
        }

        private void _btnViewDeploymentHistory_Click(object sender, EventArgs e)
        {
            using (var dlg = new ViewDeploymentHistoryForm(_diFactory, _batchRequest, Path.GetDirectoryName(_txtRequestFileName.Text)))
            {
                dlg.ShowDialog();                
            }
        }

        private void _btnExportHistory_Click(object sender, EventArgs e)
        {
            using(var dlg = new ExportHistoryForm(_diFactory, _batchRequest, Path.GetDirectoryName(_txtRequestFileName.Text)))
            {
                dlg.ShowDialog();
            }
        }
    }
}
