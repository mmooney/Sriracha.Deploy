namespace Sriracha.Deploy.Offline
{
    partial class RunDeploymentForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._grdStatus = new System.Windows.Forms.DataGridView();
            this._btnStart = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._colStatusIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this._colBuildDisplayValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colMachineName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colStatusDisplayValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colViewDetails = new System.Windows.Forms.DataGridViewLinkColumn();
            ((System.ComponentModel.ISupportInitialize)(this._grdStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // _grdStatus
            // 
            this._grdStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._grdStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._grdStatus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._colStatusIcon,
            this._colBuildDisplayValue,
            this._colMachineName,
            this._colStatusDisplayValue,
            this._colViewDetails});
            this._grdStatus.Location = new System.Drawing.Point(12, 56);
            this._grdStatus.Name = "_grdStatus";
            this._grdStatus.RowHeadersVisible = false;
            this._grdStatus.Size = new System.Drawing.Size(850, 431);
            this._grdStatus.TabIndex = 0;
            this._grdStatus.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grdStatus_CellContentClick);
            // 
            // _btnStart
            // 
            this._btnStart.Location = new System.Drawing.Point(706, 493);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(75, 23);
            this._btnStart.TabIndex = 1;
            this._btnStart.Text = "Start";
            this._btnStart.UseVisualStyleBackColor = true;
            this._btnStart.Click += new System.EventHandler(this._btnStart_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Enabled = false;
            this._btnCancel.Location = new System.Drawing.Point(787, 493);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _colStatusIcon
            // 
            this._colStatusIcon.DataPropertyName = "StatusIconImage";
            this._colStatusIcon.HeaderText = "";
            this._colStatusIcon.Name = "_colStatusIcon";
            this._colStatusIcon.Width = 25;
            // 
            // _colBuildDisplayValue
            // 
            this._colBuildDisplayValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._colBuildDisplayValue.DataPropertyName = "BuildDisplayValue";
            this._colBuildDisplayValue.HeaderText = "Build";
            this._colBuildDisplayValue.Name = "_colBuildDisplayValue";
            this._colBuildDisplayValue.ReadOnly = true;
            this._colBuildDisplayValue.Width = 55;
            // 
            // _colMachineName
            // 
            this._colMachineName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this._colMachineName.DataPropertyName = "MachineName";
            this._colMachineName.HeaderText = "Machine";
            this._colMachineName.Name = "_colMachineName";
            this._colMachineName.ReadOnly = true;
            this._colMachineName.Width = 73;
            // 
            // _colStatusDisplayValue
            // 
            this._colStatusDisplayValue.DataPropertyName = "StatusDisplayValue";
            this._colStatusDisplayValue.HeaderText = "Status";
            this._colStatusDisplayValue.Name = "_colStatusDisplayValue";
            this._colStatusDisplayValue.ReadOnly = true;
            // 
            // _colViewDetails
            // 
            this._colViewDetails.HeaderText = "Details";
            this._colViewDetails.Name = "_colViewDetails";
            this._colViewDetails.ReadOnly = true;
            this._colViewDetails.Text = "Details";
            this._colViewDetails.UseColumnTextForLinkValue = true;
            this._colViewDetails.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // RunDeploymentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 525);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnStart);
            this.Controls.Add(this._grdStatus);
            this.Name = "RunDeploymentForm";
            this.Text = "Run Deployment";
            ((System.ComponentModel.ISupportInitialize)(this._grdStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _grdStatus;
        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.DataGridViewImageColumn _colStatusIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colBuildDisplayValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colMachineName;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colStatusDisplayValue;
        private System.Windows.Forms.DataGridViewLinkColumn _colViewDetails;
    }
}