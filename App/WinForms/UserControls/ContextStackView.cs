namespace WinForms.UserControls {
    using Pyro;
    using Pyro.Exec;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
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

        private ListViewItem MakeCodeViewItem(int n, object item) {
            var row = new ListViewItem(n.ToString());
            DataStackView.AddSubItem(row, MainForm.Registry.ToPiScript(item));
            DataStackView.AddSubItem(row, DataStackView.GetType(item));
            if (item as IReflected != null) {
                DataStackView.AddSubItem(row, (item as IReflected).SelfBase.Id.ToString());
            }
            return row;
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
            context.OnIpChanged += OnIpChanged;
        }

        private void RemoveEvents(Continuation continuation) {
            continuation.OnScopeChanged -= UpdateCont;
            continuation.OnLeave -= RemoveEvents;
            continuation.OnIpChanged -= OnIpChanged;
        }

        private void OnIpChanged(Continuation continuation, int last, int current) {
            codeView.Items[last].BackColor = Color.White;
            if (current < codeView.Items.Count) {
                codeView.Items[current].BackColor = Color.LightGray;
            }
        }

        private void UpdateCont(Continuation continuation) {
            Executor.PushContext(continuation);
            UpdateContinuation();
        }

        private void UpdateContinuation() {
            codeView.Items.Clear();
            var current = Executor.Current;
            if (current == null) {
                MessageBox.Show("Nothing to Continue", "Empty Context Stack", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var row = new ListViewItem(n.ToString());
            if (obj == null) {
                DataStackView.AddSubItem(row, "<null>");
                return;
            }

            DataStackView.AddSubItem(row, MainForm.Registry.ToPiScript(obj));
            DataStackView.AddSubItem(row, (obj as IReflected)?.SelfBase.Id.ToString());
            codeView.Items.Add(row);
        }

        private void UpdateContextStack() {
            contextStack.Items.Clear();
            var stack = Executor.ContextStack;
            if (stack == null || stack.Count == 0) {
                return;
            }
        }

        private void AddContinuationToStack(int v, Continuation item) {
        }

        private void AddContinuation(int num, Continuation continuation) {
            UpdateScope(continuation);
            UpdateCode(continuation);
        }

        private void UpdateScope(Continuation continuation) {
            scopeView.Items.Clear();
            foreach (var scoped in continuation.Scope) {
                AddScopeItem(scoped.Key, scoped.Value);

            }
        }
        private void UpdateCode(Continuation continuation) {
            codeView.Items.Clear();
            int n = 0;
            foreach (var op in continuation.Code) {
                AddCodeItem(n++, op);
            }
        }

        private void stepPreviousClicked(object sender, EventArgs e) {
            MainForm.Executor.Prev();
        }

        private void stepNextClicked(object sender, EventArgs e) {
            var exec = MainForm.Executor;
            var cont = MainForm.Executor.Current;
            exec.Next();
            if (cont == null) {
                return;
            }
            if (cont.Ip > cont.Code.Count) {
                MessageBox.Show("End of Continuation", "At End", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void UpdateView() {
            UpdateContextStack();
            UpdateContinuation();
        }
    }
}

