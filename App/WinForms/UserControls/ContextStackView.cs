namespace WinForms.UserControls {
    using Pyro.Exec;
    using System;
    using System.Windows.Forms;

    public partial class ContextStackView
        : UserControlBase
        , IUserControlCommon {
        public ContextStackView() {
            InitializeComponent();
        }

        private void ContextStackView_Load(object sender, EventArgs e) {
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
        }

        internal void Show(Continuation cont) {
            var n = 0;
            foreach (var item in cont.Code) {
                AddItem(n++, item);
            }
        }

        private void AddItem(int n, object obj) {
            var item = new ListViewItem();
            var subs = new ListViewItem.ListViewSubItem[3];
            if (obj == null) {
                subs[0] = NewSubItem(item, n, "null");
                return;
            }

            subs[1] = NewSubItem(item, 1, Registry.ToPiScript(obj));
            subs[2] = NewSubItem(item, 2, obj.GetType().ToString());
            listView1.Items.Add(item);
        }

        private ListViewItem.ListViewSubItem NewSubItem(ListViewItem item, int n, string text) {
            return new ListViewItem.ListViewSubItem(item, text);
        }

        public override void Construct(IMainForm mainForm) {
            throw new NotImplementedException();
        }

        public override void Render() {
            throw new NotImplementedException();
        }
    }
}

