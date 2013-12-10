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
    public partial class ErrorForm : Form
    {
        public ErrorForm(string message, Exception err)
        {
            InitializeComponent();
            _lblErrorMessage.Text = message;
            _txtErrorDetails.Text = err.ToString();
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
