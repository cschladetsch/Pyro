namespace WinForms.UserControls
{
    using System;

    public partial class PiDebugger
        : UserControlBase
    {
        //private MainForm _main;

        public PiDebugger()
        {
            InitializeComponent();
        }

        private void PiDebugger_Load(object sender, EventArgs e)
        {
        }

        public override void Clear()
        {
            listView1.Items.Clear();
        }

        public void Restart()
        {
            _contextStackView.Clear();
            _contextStackView.Show(_Exec.Context());
        }
    }
}
