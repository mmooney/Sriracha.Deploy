using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Offline
{
    public partial class ComponentSelectionControl : UserControl
    {
        private DeployBatchRequestItem _batchRequestItem;

        public ComponentSelectionControl(DeployBatchRequestItem item)
        {
            InitializeComponent();
            _batchRequestItem = item;
        }

        private void ComponentSelectionControl_Load(object sender, EventArgs e)
        {
            _chkComponentChecked.Text = _batchRequestItem.Build.DisplayValue;
            _chkMachineCheckboxList.ValueMember = "MachineName";
            _chkMachineCheckboxList.DataSource = _batchRequestItem.MachineList;
        }

        private void _btnSelectAllMachines_Click(object sender, EventArgs e)
        {  
            bool selected = true;
            CheckAllMachines(selected);
        }

        private void CheckAllMachines(bool selected)
        {
            for (int i = 0; i < _batchRequestItem.MachineList.Count; i++)
            {
                _chkMachineCheckboxList.SetItemChecked(i, selected);
            }
        }

        private bool AreNoMachinesSelected()
        {
            return (_chkMachineCheckboxList.CheckedItems.Count == 0);
        }

        private bool AreAllMachinesSelected()
        {
            return (_chkMachineCheckboxList.CheckedItems.Count == _chkMachineCheckboxList.Items.Count);
        }

        private void _btnClearAllMachines_Click(object sender, EventArgs e)
        {
            CheckAllMachines(false);
        }

        private void _chkMachineCheckboxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
            if (this.AreAllMachinesSelected())
            {
                _chkComponentChecked.CheckState = CheckState.Checked;
            }
            else if(this.AreNoMachinesSelected())
            {
                _chkComponentChecked.CheckState = CheckState.Unchecked;
            }
            else 
            {
                _chkComponentChecked.CheckState = CheckState.Indeterminate;
            }
            _chkComponentChecked.CheckStateChanged += _chkComponentChecked_CheckStateChanged;
        }

        private void _chkComponentChecked_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.AreAllMachinesSelected())
            {
                _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
                    CheckAllMachines(false);
                    _chkComponentChecked.CheckState = CheckState.Unchecked;
                _chkComponentChecked.CheckStateChanged += _chkComponentChecked_CheckStateChanged;
            }
            else if (this.AreNoMachinesSelected())
            {
                _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
                    CheckAllMachines(true);
                    _chkComponentChecked.CheckState = CheckState.Checked;
                _chkComponentChecked.CheckStateChanged += _chkComponentChecked_CheckStateChanged;
            }
            else
            {
                _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
                    //Some machines wree selected
                    CheckAllMachines(true);
                    _chkComponentChecked.CheckState = CheckState.Checked;
                _chkComponentChecked.CheckStateChanged += _chkComponentChecked_CheckStateChanged;
            }
        }

    }
}
