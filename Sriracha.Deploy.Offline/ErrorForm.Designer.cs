namespace Sriracha.Deploy.Offline
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this._pnlErrorIcon = new System.Windows.Forms.Panel();
            this._lblErrorMessageHeader = new System.Windows.Forms.Label();
            this._lblErrorMessage = new System.Windows.Forms.Label();
            this._txtErrorDetails = new System.Windows.Forms.TextBox();
            this._lblErrorDetailsHeader = new System.Windows.Forms.Label();
            this._btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _pnlErrorIcon
            // 
            this._pnlErrorIcon.BackgroundImage = global::Sriracha.Deploy.Offline.Properties.Resources.StatusAnnotations_Critical_32xLG_color;
            this._pnlErrorIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._pnlErrorIcon.Location = new System.Drawing.Point(13, 13);
            this._pnlErrorIcon.Name = "_pnlErrorIcon";
            this._pnlErrorIcon.Size = new System.Drawing.Size(32, 32);
            this._pnlErrorIcon.TabIndex = 0;
            // 
            // _lblErrorMessageHeader
            // 
            this._lblErrorMessageHeader.AutoSize = true;
            this._lblErrorMessageHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblErrorMessageHeader.Location = new System.Drawing.Point(51, 13);
            this._lblErrorMessageHeader.Name = "_lblErrorMessageHeader";
            this._lblErrorMessageHeader.Size = new System.Drawing.Size(164, 13);
            this._lblErrorMessageHeader.TabIndex = 1;
            this._lblErrorMessageHeader.Text = "The following error ocurred:";
            // 
            // _lblErrorMessage
            // 
            this._lblErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblErrorMessage.AutoEllipsis = true;
            this._lblErrorMessage.Location = new System.Drawing.Point(51, 26);
            this._lblErrorMessage.Name = "_lblErrorMessage";
            this._lblErrorMessage.Size = new System.Drawing.Size(467, 35);
            this._lblErrorMessage.TabIndex = 2;
            this._lblErrorMessage.Text = resources.GetString("_lblErrorMessage.Text");
            // 
            // _txtErrorDetails
            // 
            this._txtErrorDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtErrorDetails.Location = new System.Drawing.Point(13, 91);
            this._txtErrorDetails.Multiline = true;
            this._txtErrorDetails.Name = "_txtErrorDetails";
            this._txtErrorDetails.ReadOnly = true;
            this._txtErrorDetails.Size = new System.Drawing.Size(505, 183);
            this._txtErrorDetails.TabIndex = 3;
            // 
            // _lblErrorDetailsHeader
            // 
            this._lblErrorDetailsHeader.AutoSize = true;
            this._lblErrorDetailsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblErrorDetailsHeader.Location = new System.Drawing.Point(12, 75);
            this._lblErrorDetailsHeader.Name = "_lblErrorDetailsHeader";
            this._lblErrorDetailsHeader.Size = new System.Drawing.Size(81, 13);
            this._lblErrorDetailsHeader.TabIndex = 4;
            this._lblErrorDetailsHeader.Text = "Error Details:";
            // 
            // _btnOK
            // 
            this._btnOK.Location = new System.Drawing.Point(443, 279);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 5;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 314);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._lblErrorDetailsHeader);
            this.Controls.Add(this._txtErrorDetails);
            this.Controls.Add(this._lblErrorMessage);
            this.Controls.Add(this._lblErrorMessageHeader);
            this.Controls.Add(this._pnlErrorIcon);
            this.Name = "ErrorForm";
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _pnlErrorIcon;
        private System.Windows.Forms.Label _lblErrorMessageHeader;
        private System.Windows.Forms.Label _lblErrorMessage;
        private System.Windows.Forms.TextBox _txtErrorDetails;
        private System.Windows.Forms.Label _lblErrorDetailsHeader;
        private System.Windows.Forms.Button _btnOK;
    }
}