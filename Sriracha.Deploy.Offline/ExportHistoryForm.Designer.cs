namespace Sriracha.Deploy.Offline
{
    partial class ExportHistoryForm
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
            this._lblFileNameHeader = new System.Windows.Forms.Label();
            this._txtOutputFileName = new System.Windows.Forms.TextBox();
            this._chkPublishToWebsite = new System.Windows.Forms.CheckBox();
            this._btnTestUrl = new System.Windows.Forms.Button();
            this._txtPublishUrl = new System.Windows.Forms.TextBox();
            this._lblRootUrlHeader = new System.Windows.Forms.Label();
            this._lblUserNameHeader = new System.Windows.Forms.Label();
            this._txtUserName = new System.Windows.Forms.TextBox();
            this._lblPasswordHeader = new System.Windows.Forms.Label();
            this._txtPassword = new System.Windows.Forms.TextBox();
            this._btnExport = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lblFileNameHeader
            // 
            this._lblFileNameHeader.AutoSize = true;
            this._lblFileNameHeader.Location = new System.Drawing.Point(13, 13);
            this._lblFileNameHeader.Name = "_lblFileNameHeader";
            this._lblFileNameHeader.Size = new System.Drawing.Size(92, 13);
            this._lblFileNameHeader.TabIndex = 0;
            this._lblFileNameHeader.Text = "Output File Name:";
            // 
            // _txtOutputFileName
            // 
            this._txtOutputFileName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._txtOutputFileName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this._txtOutputFileName.Location = new System.Drawing.Point(111, 10);
            this._txtOutputFileName.Name = "_txtOutputFileName";
            this._txtOutputFileName.Size = new System.Drawing.Size(332, 20);
            this._txtOutputFileName.TabIndex = 1;
            this._txtOutputFileName.Text = "_txtOutputFileName";
            // 
            // _chkPublishToWebsite
            // 
            this._chkPublishToWebsite.AutoSize = true;
            this._chkPublishToWebsite.Checked = true;
            this._chkPublishToWebsite.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkPublishToWebsite.Location = new System.Drawing.Point(16, 42);
            this._chkPublishToWebsite.Name = "_chkPublishToWebsite";
            this._chkPublishToWebsite.Size = new System.Drawing.Size(226, 17);
            this._chkPublishToWebsite.TabIndex = 2;
            this._chkPublishToWebsite.Text = "Publish History Back To Sriracha Website:";
            this._chkPublishToWebsite.UseVisualStyleBackColor = true;
            this._chkPublishToWebsite.CheckedChanged += new System.EventHandler(this._chkPublishToWebsite_CheckedChanged);
            // 
            // _btnTestUrl
            // 
            this._btnTestUrl.Location = new System.Drawing.Point(449, 63);
            this._btnTestUrl.Name = "_btnTestUrl";
            this._btnTestUrl.Size = new System.Drawing.Size(75, 23);
            this._btnTestUrl.TabIndex = 9;
            this._btnTestUrl.Text = "Test";
            this._btnTestUrl.UseVisualStyleBackColor = true;
            this._btnTestUrl.Click += new System.EventHandler(this._btnTestUrl_Click);
            // 
            // _txtPublishUrl
            // 
            this._txtPublishUrl.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._txtPublishUrl.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this._txtPublishUrl.Location = new System.Drawing.Point(111, 65);
            this._txtPublishUrl.Name = "_txtPublishUrl";
            this._txtPublishUrl.Size = new System.Drawing.Size(332, 20);
            this._txtPublishUrl.TabIndex = 4;
            this._txtPublishUrl.Text = "_txtPublishUrl";
            // 
            // _lblRootUrlHeader
            // 
            this._lblRootUrlHeader.AutoSize = true;
            this._lblRootUrlHeader.Location = new System.Drawing.Point(56, 68);
            this._lblRootUrlHeader.Name = "_lblRootUrlHeader";
            this._lblRootUrlHeader.Size = new System.Drawing.Size(49, 13);
            this._lblRootUrlHeader.TabIndex = 3;
            this._lblRootUrlHeader.Text = "Root Url:";
            // 
            // _lblUserNameHeader
            // 
            this._lblUserNameHeader.AutoSize = true;
            this._lblUserNameHeader.Location = new System.Drawing.Point(40, 94);
            this._lblUserNameHeader.Name = "_lblUserNameHeader";
            this._lblUserNameHeader.Size = new System.Drawing.Size(63, 13);
            this._lblUserNameHeader.TabIndex = 5;
            this._lblUserNameHeader.Text = "User Name:";
            // 
            // _txtUserName
            // 
            this._txtUserName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._txtUserName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this._txtUserName.Location = new System.Drawing.Point(111, 91);
            this._txtUserName.Name = "_txtUserName";
            this._txtUserName.Size = new System.Drawing.Size(332, 20);
            this._txtUserName.TabIndex = 6;
            // 
            // _lblPasswordHeader
            // 
            this._lblPasswordHeader.AutoSize = true;
            this._lblPasswordHeader.Location = new System.Drawing.Point(47, 120);
            this._lblPasswordHeader.Name = "_lblPasswordHeader";
            this._lblPasswordHeader.Size = new System.Drawing.Size(56, 13);
            this._lblPasswordHeader.TabIndex = 7;
            this._lblPasswordHeader.Text = "Password:";
            // 
            // _txtPassword
            // 
            this._txtPassword.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._txtPassword.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this._txtPassword.Location = new System.Drawing.Point(111, 117);
            this._txtPassword.Name = "_txtPassword";
            this._txtPassword.Size = new System.Drawing.Size(332, 20);
            this._txtPassword.TabIndex = 8;
            this._txtPassword.UseSystemPasswordChar = true;
            // 
            // _btnExport
            // 
            this._btnExport.Location = new System.Drawing.Point(368, 153);
            this._btnExport.Name = "_btnExport";
            this._btnExport.Size = new System.Drawing.Size(75, 23);
            this._btnExport.TabIndex = 10;
            this._btnExport.Text = "Export";
            this._btnExport.UseVisualStyleBackColor = true;
            this._btnExport.Click += new System.EventHandler(this._btnExport_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Location = new System.Drawing.Point(449, 153);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 11;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // ExportHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(543, 188);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnExport);
            this.Controls.Add(this._lblPasswordHeader);
            this.Controls.Add(this._txtPassword);
            this.Controls.Add(this._lblUserNameHeader);
            this.Controls.Add(this._txtUserName);
            this.Controls.Add(this._lblRootUrlHeader);
            this.Controls.Add(this._btnTestUrl);
            this.Controls.Add(this._txtPublishUrl);
            this.Controls.Add(this._chkPublishToWebsite);
            this.Controls.Add(this._txtOutputFileName);
            this.Controls.Add(this._lblFileNameHeader);
            this.Name = "ExportHistoryForm";
            this.Text = "Export Deployment History";
            this.Load += new System.EventHandler(this.ExportHistoryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblFileNameHeader;
        private System.Windows.Forms.TextBox _txtOutputFileName;
        private System.Windows.Forms.CheckBox _chkPublishToWebsite;
        private System.Windows.Forms.Button _btnTestUrl;
        private System.Windows.Forms.TextBox _txtPublishUrl;
        private System.Windows.Forms.Label _lblRootUrlHeader;
        private System.Windows.Forms.Label _lblUserNameHeader;
        private System.Windows.Forms.TextBox _txtUserName;
        private System.Windows.Forms.Label _lblPasswordHeader;
        private System.Windows.Forms.TextBox _txtPassword;
        private System.Windows.Forms.Button _btnExport;
        private System.Windows.Forms.Button _btnCancel;
    }
}