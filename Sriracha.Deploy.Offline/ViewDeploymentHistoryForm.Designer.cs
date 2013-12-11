namespace Sriracha.Deploy.Offline
{
    partial class ViewDeploymentHistoryForm
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
            this._lblLabelHeader = new System.Windows.Forms.Label();
            this._lblLabel = new System.Windows.Forms.Label();
            this._lblDirectoryHeader = new System.Windows.Forms.Label();
            this._lblDirectory = new System.Windows.Forms.Label();
            this._grdHistory = new System.Windows.Forms.DataGridView();
            this._btnClose = new System.Windows.Forms.Button();
            this._colRunStatusIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this._colRunStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colStartedDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colCompletedDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colViewRerun = new System.Windows.Forms.DataGridViewLinkColumn();
            ((System.ComponentModel.ISupportInitialize)(this._grdHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // _lblLabelHeader
            // 
            this._lblLabelHeader.AutoSize = true;
            this._lblLabelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblLabelHeader.Location = new System.Drawing.Point(12, 9);
            this._lblLabelHeader.Name = "_lblLabelHeader";
            this._lblLabelHeader.Size = new System.Drawing.Size(42, 13);
            this._lblLabelHeader.TabIndex = 0;
            this._lblLabelHeader.Text = "Label:";
            // 
            // _lblLabel
            // 
            this._lblLabel.AutoSize = true;
            this._lblLabel.Location = new System.Drawing.Point(80, 9);
            this._lblLabel.Name = "_lblLabel";
            this._lblLabel.Size = new System.Drawing.Size(49, 13);
            this._lblLabel.TabIndex = 1;
            this._lblLabel.Text = "_lblLabel";
            // 
            // _lblDirectoryHeader
            // 
            this._lblDirectoryHeader.AutoSize = true;
            this._lblDirectoryHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDirectoryHeader.Location = new System.Drawing.Point(12, 31);
            this._lblDirectoryHeader.Name = "_lblDirectoryHeader";
            this._lblDirectoryHeader.Size = new System.Drawing.Size(62, 13);
            this._lblDirectoryHeader.TabIndex = 2;
            this._lblDirectoryHeader.Text = "Directory:";
            // 
            // _lblDirectory
            // 
            this._lblDirectory.AutoSize = true;
            this._lblDirectory.Location = new System.Drawing.Point(80, 31);
            this._lblDirectory.Name = "_lblDirectory";
            this._lblDirectory.Size = new System.Drawing.Size(65, 13);
            this._lblDirectory.TabIndex = 3;
            this._lblDirectory.Text = "_lblDirectory";
            // 
            // _grdHistory
            // 
            this._grdHistory.AllowUserToAddRows = false;
            this._grdHistory.AllowUserToDeleteRows = false;
            this._grdHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._grdHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._grdHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._colRunStatusIcon,
            this._colRunStatus,
            this._colStartedDate,
            this._colCompletedDate,
            this._colViewRerun});
            this._grdHistory.Location = new System.Drawing.Point(15, 73);
            this._grdHistory.Name = "_grdHistory";
            this._grdHistory.ReadOnly = true;
            this._grdHistory.RowHeadersVisible = false;
            this._grdHistory.Size = new System.Drawing.Size(819, 369);
            this._grdHistory.TabIndex = 4;
            this._grdHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grdHistory_CellContentClick);
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.Location = new System.Drawing.Point(759, 449);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(75, 23);
            this._btnClose.TabIndex = 5;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
            // 
            // _colRunStatusIcon
            // 
            this._colRunStatusIcon.DataPropertyName = "StatusIconImage";
            this._colRunStatusIcon.HeaderText = "";
            this._colRunStatusIcon.Name = "_colRunStatusIcon";
            this._colRunStatusIcon.ReadOnly = true;
            this._colRunStatusIcon.Width = 15;
            // 
            // _colRunStatus
            // 
            this._colRunStatus.DataPropertyName = "StatusDisplayValue";
            this._colRunStatus.HeaderText = "Status";
            this._colRunStatus.Name = "_colRunStatus";
            this._colRunStatus.ReadOnly = true;
            // 
            // _colStartedDate
            // 
            this._colStartedDate.DataPropertyName = "StartedDateDisplayValue";
            this._colStartedDate.HeaderText = "Started Date";
            this._colStartedDate.Name = "_colStartedDate";
            this._colStartedDate.ReadOnly = true;
            // 
            // _colCompletedDate
            // 
            this._colCompletedDate.DataPropertyName = "StartedDateDisplayValue";
            this._colCompletedDate.HeaderText = "Completed Date";
            this._colCompletedDate.Name = "_colCompletedDate";
            this._colCompletedDate.ReadOnly = true;
            // 
            // _colViewRerun
            // 
            this._colViewRerun.HeaderText = "Details";
            this._colViewRerun.Name = "_colViewRerun";
            this._colViewRerun.ReadOnly = true;
            this._colViewRerun.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this._colViewRerun.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this._colViewRerun.Text = "View/Re-run";
            this._colViewRerun.UseColumnTextForLinkValue = true;
            this._colViewRerun.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // ViewDeploymentHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnClose;
            this.ClientSize = new System.Drawing.Size(846, 484);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._grdHistory);
            this.Controls.Add(this._lblDirectory);
            this.Controls.Add(this._lblDirectoryHeader);
            this.Controls.Add(this._lblLabel);
            this.Controls.Add(this._lblLabelHeader);
            this.Name = "ViewDeploymentHistoryForm";
            this.Text = "Deployment History";
            this.Load += new System.EventHandler(this.ViewDeploymentHistoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._grdHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblLabelHeader;
        private System.Windows.Forms.Label _lblLabel;
        private System.Windows.Forms.Label _lblDirectoryHeader;
        private System.Windows.Forms.Label _lblDirectory;
        private System.Windows.Forms.DataGridView _grdHistory;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.DataGridViewImageColumn _colRunStatusIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colRunStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colStartedDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colCompletedDate;
        private System.Windows.Forms.DataGridViewLinkColumn _colViewRerun;
    }
}