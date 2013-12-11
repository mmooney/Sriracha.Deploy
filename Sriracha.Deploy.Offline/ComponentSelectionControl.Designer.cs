namespace Sriracha.Deploy.Offline
{
    partial class ComponentSelectionControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._chkMachineCheckboxList = new System.Windows.Forms.CheckedListBox();
            this._lblMachineHeader = new System.Windows.Forms.Label();
            this._btnSelectAllMachines = new System.Windows.Forms.Button();
            this._btnClearAllMachines = new System.Windows.Forms.Button();
            this._chkComponentChecked = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _chkMachineCheckboxList
            // 
            this._chkMachineCheckboxList.CheckOnClick = true;
            this._chkMachineCheckboxList.FormattingEnabled = true;
            this._chkMachineCheckboxList.Items.AddRange(new object[] {
            "test",
            "test 1",
            "test 2",
            "test 3",
            "test 4",
            "test 5",
            "test 6",
            "test 7"});
            this._chkMachineCheckboxList.Location = new System.Drawing.Point(40, 52);
            this._chkMachineCheckboxList.MultiColumn = true;
            this._chkMachineCheckboxList.Name = "_chkMachineCheckboxList";
            this._chkMachineCheckboxList.Size = new System.Drawing.Size(505, 49);
            this._chkMachineCheckboxList.TabIndex = 1;
            this._chkMachineCheckboxList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._chkMachineCheckboxList_ItemCheck);
            // 
            // _lblMachineHeader
            // 
            this._lblMachineHeader.AutoSize = true;
            this._lblMachineHeader.Location = new System.Drawing.Point(37, 36);
            this._lblMachineHeader.Name = "_lblMachineHeader";
            this._lblMachineHeader.Size = new System.Drawing.Size(56, 13);
            this._lblMachineHeader.TabIndex = 2;
            this._lblMachineHeader.Text = "Machines:";
            // 
            // _btnSelectAllMachines
            // 
            this._btnSelectAllMachines.Location = new System.Drawing.Point(389, 12);
            this._btnSelectAllMachines.Name = "_btnSelectAllMachines";
            this._btnSelectAllMachines.Size = new System.Drawing.Size(75, 23);
            this._btnSelectAllMachines.TabIndex = 3;
            this._btnSelectAllMachines.Text = "Select All";
            this._btnSelectAllMachines.UseVisualStyleBackColor = true;
            this._btnSelectAllMachines.Click += new System.EventHandler(this._btnSelectAllMachines_Click);
            // 
            // _btnClearAllMachines
            // 
            this._btnClearAllMachines.Location = new System.Drawing.Point(470, 12);
            this._btnClearAllMachines.Name = "_btnClearAllMachines";
            this._btnClearAllMachines.Size = new System.Drawing.Size(75, 23);
            this._btnClearAllMachines.TabIndex = 4;
            this._btnClearAllMachines.Text = "Clear All";
            this._btnClearAllMachines.UseVisualStyleBackColor = true;
            this._btnClearAllMachines.Click += new System.EventHandler(this._btnClearAllMachines_Click);
            // 
            // _chkComponentChecked
            // 
            this._chkComponentChecked.AutoSize = true;
            this._chkComponentChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._chkComponentChecked.Location = new System.Drawing.Point(13, 12);
            this._chkComponentChecked.Name = "_chkComponentChecked";
            this._chkComponentChecked.Size = new System.Drawing.Size(89, 17);
            this._chkComponentChecked.TabIndex = 5;
            this._chkComponentChecked.Text = "checkBox1";
            this._chkComponentChecked.ThreeState = true;
            this._chkComponentChecked.UseVisualStyleBackColor = true;
            this._chkComponentChecked.CheckStateChanged += new System.EventHandler(this._chkComponentChecked_CheckStateChanged);
            // 
            // ComponentSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._chkComponentChecked);
            this.Controls.Add(this._btnClearAllMachines);
            this.Controls.Add(this._btnSelectAllMachines);
            this.Controls.Add(this._lblMachineHeader);
            this.Controls.Add(this._chkMachineCheckboxList);
            this.Name = "ComponentSelectionControl";
            this.Size = new System.Drawing.Size(585, 111);
            this.Load += new System.EventHandler(this.ComponentSelectionControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox _chkMachineCheckboxList;
        private System.Windows.Forms.Label _lblMachineHeader;
        private System.Windows.Forms.Button _btnSelectAllMachines;
        private System.Windows.Forms.Button _btnClearAllMachines;
        private System.Windows.Forms.CheckBox _chkComponentChecked;
    }
}
