namespace Sriracha.Deploy.Offline
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._lblRequestFileName = new System.Windows.Forms.Label();
            this._txtRequestFileName = new System.Windows.Forms.TextBox();
            this._btnRequestFileNameBrowse = new System.Windows.Forms.Button();
            this._pnlAllComponents = new System.Windows.Forms.FlowLayoutPanel();
            this._btnSelectAll = new System.Windows.Forms.Button();
            this._btnClearAll = new System.Windows.Forms.Button();
            this._btnSelectMachines = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lblRequestFileName
            // 
            this._lblRequestFileName.AutoSize = true;
            this._lblRequestFileName.Location = new System.Drawing.Point(13, 13);
            this._lblRequestFileName.Name = "_lblRequestFileName";
            this._lblRequestFileName.Size = new System.Drawing.Size(97, 13);
            this._lblRequestFileName.TabIndex = 0;
            this._lblRequestFileName.Text = "Request File Name";
            // 
            // _txtRequestFileName
            // 
            this._txtRequestFileName.Location = new System.Drawing.Point(117, 13);
            this._txtRequestFileName.Name = "_txtRequestFileName";
            this._txtRequestFileName.ReadOnly = true;
            this._txtRequestFileName.Size = new System.Drawing.Size(382, 20);
            this._txtRequestFileName.TabIndex = 1;
            // 
            // _btnRequestFileNameBrowse
            // 
            this._btnRequestFileNameBrowse.Location = new System.Drawing.Point(525, 9);
            this._btnRequestFileNameBrowse.Name = "_btnRequestFileNameBrowse";
            this._btnRequestFileNameBrowse.Size = new System.Drawing.Size(75, 23);
            this._btnRequestFileNameBrowse.TabIndex = 2;
            this._btnRequestFileNameBrowse.Text = "&Browse";
            this._btnRequestFileNameBrowse.UseVisualStyleBackColor = true;
            this._btnRequestFileNameBrowse.Click += new System.EventHandler(this._btnRequestFileNameBrowse_Click);
            // 
            // _pnlAllComponents
            // 
            this._pnlAllComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlAllComponents.AutoScroll = true;
            this._pnlAllComponents.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._pnlAllComponents.Location = new System.Drawing.Point(12, 79);
            this._pnlAllComponents.Name = "_pnlAllComponents";
            this._pnlAllComponents.Size = new System.Drawing.Size(706, 407);
            this._pnlAllComponents.TabIndex = 3;
            this._pnlAllComponents.WrapContents = false;
            // 
            // _btnSelectAll
            // 
            this._btnSelectAll.Location = new System.Drawing.Point(198, 39);
            this._btnSelectAll.Name = "_btnSelectAll";
            this._btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this._btnSelectAll.TabIndex = 4;
            this._btnSelectAll.Text = "Select All";
            this._btnSelectAll.UseVisualStyleBackColor = true;
            this._btnSelectAll.Click += new System.EventHandler(this._btnSelectAll_Click);
            // 
            // _btnClearAll
            // 
            this._btnClearAll.Location = new System.Drawing.Point(279, 39);
            this._btnClearAll.Name = "_btnClearAll";
            this._btnClearAll.Size = new System.Drawing.Size(75, 23);
            this._btnClearAll.TabIndex = 5;
            this._btnClearAll.Text = "Clear All";
            this._btnClearAll.UseVisualStyleBackColor = true;
            this._btnClearAll.Click += new System.EventHandler(this._btnClearAll_Click);
            // 
            // _btnSelectMachines
            // 
            this._btnSelectMachines.Location = new System.Drawing.Point(360, 39);
            this._btnSelectMachines.Name = "_btnSelectMachines";
            this._btnSelectMachines.Size = new System.Drawing.Size(140, 23);
            this._btnSelectMachines.TabIndex = 6;
            this._btnSelectMachines.Text = "Select Specific Machines";
            this._btnSelectMachines.UseVisualStyleBackColor = true;
            this._btnSelectMachines.Click += new System.EventHandler(this._btnSelectMachines_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 524);
            this.Controls.Add(this._btnSelectMachines);
            this.Controls.Add(this._btnClearAll);
            this.Controls.Add(this._btnSelectAll);
            this.Controls.Add(this._pnlAllComponents);
            this.Controls.Add(this._btnRequestFileNameBrowse);
            this.Controls.Add(this._txtRequestFileName);
            this.Controls.Add(this._lblRequestFileName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Sriracha Offline Deployment Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblRequestFileName;
        private System.Windows.Forms.TextBox _txtRequestFileName;
        private System.Windows.Forms.Button _btnRequestFileNameBrowse;
        private System.Windows.Forms.FlowLayoutPanel _pnlAllComponents;
        private System.Windows.Forms.Button _btnSelectAll;
        private System.Windows.Forms.Button _btnClearAll;
        private System.Windows.Forms.Button _btnSelectMachines;
    }
}

