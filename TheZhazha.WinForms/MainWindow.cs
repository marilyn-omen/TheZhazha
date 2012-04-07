using System;
using System.Windows.Forms;
using TheZhazha.Models;

namespace TheZhazha.WinForms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            Zhazha.Start();
            btnStop.Enabled = true;
        }

        private void BtnStopClick(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            Zhazha.Stop();
            btnStart.Enabled = true;
        }

        private TextGenerator _generator;
        private void button1_Click(object sender, EventArgs e)
        {
            if (_generator == null)
                _generator = new TextGenerator();
            if (_generator.IsReady)
                MessageBox.Show(_generator.GenerateDeep());
        }
    }
}
