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
            UpdateContextStack();
        }

        internal void ContextChanged(Executor executor, Continuation context) {
            if (context == null) {
                return;
            }
            UpdateContinuation();
            context.OnScopeChanged += UpdateCont;
            context.OnLeave += RemoveEvents;
        }

        private void RemoveEvents(Continuation continuation) {
            continuation.OnScopeChanged -= UpdateCont;
            continuation.OnLeave -= RemoveEvents;
        }

        private void UpdateCont(Continuation continuation) {
            UpdateContinuation();
        }

        private void UpdateContinuation() {
            codeView.Items.Clear();
            var current = Executor.Current;
            if (current == null) {
                return;
            }
            var n = 0;
            foreach (var item in current.Code) {
                AddCodeItem(n++, item);
            }

            scopeView.Items.Clear();
            foreach (var item in current.Scope) {
                AddScopeItem(item.Key, item.Value);
            }
        }

        private void AddScopeItem(string name, object value) {
            var row = new ListViewItem(name);
            DataStackView.AddSubItem(row, MainForm.Registry.ToPiScript(value));
            DataStackView.AddSubItem(row, DataStackView.GetType(value));
            scopeView.Items.Add(row);
        }

        private void AddCodeItem(int n, object obj) {
            var item = new ListViewItem();
            var subs = new ListViewItem.ListViewSubItem[3];
            if (obj == null) {
                subs[0] = NewSubItem(item, n, "null");
                return;
            }

            subs[1] = NewSubItem(item, 1, Registry.ToPiScript(obj));
            subs[2] = NewSubItem(item, 2, obj.GetType().ToString());
            codeView.Items.Add(item);
        }

        private void UpdateContextStack() {
            contextStack.Items.Clear();
            var stack = Executor.ContextStack;
            if (stack == null || stack.Count == 0) {
                return;
            }
            var n = 0;
            foreach (var item in stack) {
                AddContinuation(n++, item);
            }
        }

        private void AddContinuation(int num, Continuation continuation) {
            var item = new ListViewItem(num.ToString());
            var subs = new ListViewItem.ListViewSubItem[3];


        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            MainForm.Executor.Next();
        }

        internal void UpdateView() {
            UpdateContextStack();
            UpdateContinuation();
        }
    }
}

