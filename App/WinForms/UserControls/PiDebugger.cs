namespace WinForms.UserControls
{
    using System;
    using System.Windows.Forms;

    public partial class PiDebugger : UserControl
    {
        public PiDebugger()
        {
            InitializeComponent();
        }

        private void PiDebugger_Load(object sender, EventArgs e)
        {
        }

        public void Clear()
        {
            listView1.Items.Clear();
        }

        public void Restart()
        {
        }
    }
}
