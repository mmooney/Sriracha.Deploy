using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;

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

        public void CheckAllMachines(bool selected)
        {
            for (int i = 0; i < _batchRequestItem.MachineList.Count; i++)
            {
                _chkMachineCheckboxList.SetItemChecked(i, selected);
            }
        }

        public void CheckSpecificMachines(List<string> machineNameList)
        {
            for (int i = 0; i < _batchRequestItem.MachineList.Count; i++)
            {
                var machine = (DeployMachine)_chkMachineCheckboxList.Items[i];
                if(machineNameList.Contains(machine.MachineName, StringComparer.CurrentCultureIgnoreCase))
                {
                    _chkMachineCheckboxList.SetItemChecked(i, true);
                }
            }
        }

        private bool AreNoMachinesSelected(int? changingItemIndex=null, CheckState? changingItemValue=null)
        {
            for(int i = 0; i < _chkMachineCheckboxList.Items.Count; i++)
            {
                bool selected;
                if(changingItemIndex.HasValue && changingItemValue.HasValue
                        && changingItemIndex.Value == i)
                {
                    selected = (changingItemValue.Value == CheckState.Checked);
                }
                else 
                {
                    selected = _chkMachineCheckboxList.GetItemChecked(i);
                }
                if(selected) 
                {
                    return false;
                }   
            }
            return true;
        }

        private bool AreAllMachinesSelected(int? changingItemIndex=null, CheckState? changingItemValue=null)
        {
            for(int i = 0; i < _chkMachineCheckboxList.Items.Count; i++)
            {
                bool selected;
                if(changingItemIndex.HasValue && changingItemValue.HasValue
                        && changingItemIndex.Value == i)
                {
                    selected = (changingItemValue.Value == CheckState.Checked);
                }
                else 
                {
                    selected = _chkMachineCheckboxList.GetItemChecked(i);
                }
                if(!selected) 
                {
                    return false;
                }   
            }
            return true;
        }

        private void _btnClearAllMachines_Click(object sender, EventArgs e)
        {
            CheckAllMachines(false);
        }

        private void _chkMachineCheckboxList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
            if (this.AreAllMachinesSelected(e.Index, e.NewValue))
            {
                _chkComponentChecked.CheckState = CheckState.Checked;
            }
            else if(this.AreNoMachinesSelected(e.Index, e.NewValue))
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
            _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
            if (this.AreAllMachinesSelected())
            {
                CheckAllMachines(false);
                _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
                _chkComponentChecked.CheckState = CheckState.Unchecked;
            }
            else if (this.AreNoMachinesSelected())
            {
                CheckAllMachines(true);
                _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
                _chkComponentChecked.CheckState = CheckState.Checked;
            }
            else
            {
                //Some machines were selected
                CheckAllMachines(true);
                _chkComponentChecked.CheckStateChanged -= _chkComponentChecked_CheckStateChanged;
                _chkComponentChecked.CheckState = CheckState.Checked;
            }
            _chkComponentChecked.CheckStateChanged += _chkComponentChecked_CheckStateChanged;
        }

        public OfflineComponentSelection GetComponentSelection()
        {
            return new OfflineComponentSelection
            {
                BatchRequestItem = _batchRequestItem,
                SelectedMachineList = _chkMachineCheckboxList.CheckedItems.Cast<DeployMachine>().ToList()
            };
        }

    }
}
