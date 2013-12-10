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
    public partial class SelectMachineForm : Form
    {
        private DeployBatchRequest _batchRequest;
        public SelectMachineForm(DeployBatchRequest batchRequest)
        {
            InitializeComponent();
            _batchRequest = batchRequest;
        }

        private void SelectMachineForm_Load(object sender, EventArgs e)
        {
            var machineNameList = _batchRequest.ItemList.SelectMany(i=>i.MachineList.Select(j=>j.MachineName)).Distinct().ToList();
            _chkMachineList.DataSource = machineNameList;
        }

        private void _chkMachineList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool anySelected = false;
            for(int i = 0; i < _chkMachineList.Items.Count; i++)
            {
                if(e.Index == i)
                {
                    if(e.NewValue == CheckState.Checked)
                    {
                        anySelected = true;
                    }
                }
                else 
                {
                    if(_chkMachineList.GetItemCheckState(i) == CheckState.Checked)
                    {
                        anySelected = true;
                    }
                }
            }
            _btnOK.Enabled = anySelected;
        }

        public List<string> GetSelectedMachineNameList()
        {
            return _chkMachineList.SelectedItems.Cast<string>().ToList();
        }
    }
}
