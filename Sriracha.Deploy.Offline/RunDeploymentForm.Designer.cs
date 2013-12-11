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
            this._colStatusIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this._colBuildDisplayValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colMachineName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colStatusDisplayValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colViewDetails = new System.Windows.Forms.DataGridViewLinkColumn();
            this._btnStart = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._lblLabelHeader = new System.Windows.Forms.Label();
            this._lblLabel = new System.Windows.Forms.Label();
            this._lblStatusHeader = new System.Windows.Forms.Label();
            this._lblStatus = new System.Windows.Forms.Label();
            this._lblStartedHeader = new System.Windows.Forms.Label();
            this._lblCompletedHeader = new System.Windows.Forms.Label();
            this._lblStarted = new System.Windows.Forms.Label();
            this._lblCompleted = new System.Windows.Forms.Label();
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
            this._grdStatus.Location = new System.Drawing.Point(12, 67);
            this._grdStatus.Name = "_grdStatus";
            this._grdStatus.RowHeadersVisible = false;
            this._grdStatus.Size = new System.Drawing.Size(850, 420);
            this._grdStatus.TabIndex = 0;
            this._grdStatus.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grdStatus_CellContentClick);
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
            // _lblLabelHeader
            // 
            this._lblLabelHeader.AutoSize = true;
            this._lblLabelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblLabelHeader.Location = new System.Drawing.Point(13, 13);
            this._lblLabelHeader.Name = "_lblLabelHeader";
            this._lblLabelHeader.Size = new System.Drawing.Size(42, 13);
            this._lblLabelHeader.TabIndex = 3;
            this._lblLabelHeader.Text = "Label:";
            // 
            // _lblLabel
            // 
            this._lblLabel.AutoSize = true;
            this._lblLabel.Location = new System.Drawing.Point(78, 13);
            this._lblLabel.Name = "_lblLabel";
            this._lblLabel.Size = new System.Drawing.Size(49, 13);
            this._lblLabel.TabIndex = 4;
            this._lblLabel.Text = "_lblLabel";
            // 
            // _lblStatusHeader
            // 
            this._lblStatusHeader.AutoSize = true;
            this._lblStatusHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblStatusHeader.Location = new System.Drawing.Point(13, 36);
            this._lblStatusHeader.Name = "_lblStatusHeader";
            this._lblStatusHeader.Size = new System.Drawing.Size(47, 13);
            this._lblStatusHeader.TabIndex = 5;
            this._lblStatusHeader.Text = "Status:";
            // 
            // _lblStatus
            // 
            this._lblStatus.AutoSize = true;
            this._lblStatus.Location = new System.Drawing.Point(78, 36);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(53, 13);
            this._lblStatus.TabIndex = 6;
            this._lblStatus.Text = "_lblStatus";
            // 
            // _lblStartedHeader
            // 
            this._lblStartedHeader.AutoSize = true;
            this._lblStartedHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblStartedHeader.Location = new System.Drawing.Point(332, 13);
            this._lblStartedHeader.Name = "_lblStartedHeader";
            this._lblStartedHeader.Size = new System.Drawing.Size(52, 13);
            this._lblStartedHeader.TabIndex = 7;
            this._lblStartedHeader.Text = "Started:";
            // 
            // _lblCompletedHeader
            // 
            this._lblCompletedHeader.AutoSize = true;
            this._lblCompletedHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblCompletedHeader.Location = new System.Drawing.Point(332, 36);
            this._lblCompletedHeader.Name = "_lblCompletedHeader";
            this._lblCompletedHeader.Size = new System.Drawing.Size(70, 13);
            this._lblCompletedHeader.TabIndex = 8;
            this._lblCompletedHeader.Text = "Completed:";
            // 
            // _lblStarted
            // 
            this._lblStarted.AutoSize = true;
            this._lblStarted.Location = new System.Drawing.Point(405, 9);
            this._lblStarted.Name = "_lblStarted";
            this._lblStarted.Size = new System.Drawing.Size(57, 13);
            this._lblStarted.TabIndex = 9;
            this._lblStarted.Text = "_lblStarted";
            // 
            // _lblCompleted
            // 
            this._lblCompleted.AutoSize = true;
            this._lblCompleted.Location = new System.Drawing.Point(408, 36);
            this._lblCompleted.Name = "_lblCompleted";
            this._lblCompleted.Size = new System.Drawing.Size(73, 13);
            this._lblCompleted.TabIndex = 10;
            this._lblCompleted.Text = "_lblCompleted";
            // 
            // RunDeploymentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 525);
            this.Controls.Add(this._lblCompleted);
            this.Controls.Add(this._lblStarted);
            this.Controls.Add(this._lblCompletedHeader);
            this.Controls.Add(this._lblStartedHeader);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._lblStatusHeader);
            this.Controls.Add(this._lblLabel);
            this.Controls.Add(this._lblLabelHeader);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnStart);
            this.Controls.Add(this._grdStatus);
            this.Name = "RunDeploymentForm";
            this.Text = "Run Deployment";
            this.Load += new System.EventHandler(this.RunDeploymentForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._grdStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Label _lblLabelHeader;
        private System.Windows.Forms.Label _lblLabel;
        private System.Windows.Forms.Label _lblStatusHeader;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Label _lblStartedHeader;
        private System.Windows.Forms.Label _lblCompletedHeader;
        private System.Windows.Forms.Label _lblStarted;
        private System.Windows.Forms.Label _lblCompleted;
    }
}