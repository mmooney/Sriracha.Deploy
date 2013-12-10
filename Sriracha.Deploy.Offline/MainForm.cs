using Newtonsoft.Json;
using Sriracha.Deploy.Data.Dto.Deployment;
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

        public MainForm()
        {
            InitializeComponent();
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
                
            }
            catch(Exception err)
            {
                this.DisplayError(string.Format("Error loading batch file name \"{0}\": {1}", filePath, err.Message), err);
            }
        }

        private void DisplayError(string message, Exception err)
        {
            using(var dlg = new ErrorForm(message, err))
            {
                dlg.ShowDialog();
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
    }
}
