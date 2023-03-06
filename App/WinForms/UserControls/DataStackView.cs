namespace WinForms.UserControls {
    using System;

    public partial class DataStackView
        : UserControlBase
        , IUserControlCommon {

        public DataStackView() {
        }

        private void ContextStackView_Load(object sender, EventArgs e) {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        public override void Construct(IMainForm mainForm) {
            MainForm = mainForm;
        }
    }
}
