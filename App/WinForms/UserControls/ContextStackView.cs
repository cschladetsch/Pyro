namespace WinForms.UserControls {
    using Pyro.Exec;
    using System;
    using System.Collections.Generic;
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
            contextStack.Items.Add(item);
        }

        private ListViewItem.ListViewSubItem NewSubItem(ListViewItem item, int n, string text) {
            return new ListViewItem.ListViewSubItem(item, text);
        }

        public override void Construct(IMainForm mainForm) {
            MainForm = mainForm;
            Executor.OnContextStackChanged += ContextStackChanged;
            Executor.OnContinuationChanged += ContextChanged;

        }

        public override void Render() {
        }

        internal void ContextStackChanged(Executor executor, List<Continuation> contexts) {
            Console.WriteLine(">>> Context stack changed");
            UpdateContextStack();
        }

        private void UpdateContextStack() {
        }

        private void UpdateDataStack() {
            contextStack.Items.Clear();
            var n = 0;
            foreach (var item in MainForm.Executor.DataStack) {
                AddItem(n++, item);
            }
        }

        internal void ContextChanged(Executor executor, Continuation context) {
            Console.WriteLine(">>> context changed");
            UpdateContext();
        }

        private void UpdateContext() {
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            MainForm.Executor.Next();
        }

        internal void UpdateView() {
            UpdateDataStack();
            UpdateContextStack();
            UpdateContext();
        }
    }
}

