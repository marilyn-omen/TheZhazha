using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheZhazha.WinForms
{
    public partial class SetMasterHandleForm : Form
    {
        public string MasterHandle { get; set; }

        public SetMasterHandleForm()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            textBoxHandle.Text = MasterHandle;
            textBoxHandle.Focus();
            textBoxHandle.SelectAll();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            MasterHandle = textBoxHandle.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
