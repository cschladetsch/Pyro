namespace WinForms {
    using System;
    using System.Windows.Forms;

    public partial class DataStackView
        : UserControl {
        private MainForm _main;

        public DataStackView() {
            InitializeComponent();
        }

        private void ContextStackView_Load(object sender, EventArgs e) {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        public void Clear() {
            listView1.Items.Clear();
        }

        public void Construct(MainForm mainForm) {
            _main = mainForm;
        }
    }
}
