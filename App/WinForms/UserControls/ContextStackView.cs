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

        public override void Construct(IMainForm mainForm) {
            MainForm = mainForm;
            Executor.OnContextStackChanged += ContextStackChanged;
            Executor.OnContinuationChanged += ContextChanged;
            SetStatusFromContinuation();
        }

        private void ContextStackChanged(Executor executor, List<Continuation> contexts) {
            UpdateContextStack(executor, contexts);
        }

        private void ContextChanged(Executor executor, Continuation previous, Continuation current) {
            if (previous != null) {
                previous.OnScopeChanged -= UpdateCont;
                previous.OnLeave -= RemoveEvents;
                previous.OnIpChanged -= OnIpChanged;
            }
            
            UpdateContinuation(current);
            
            if (current == null) {
                return;
            }
            
            current.OnScopeChanged += UpdateCont;
            current.OnLeave += RemoveEvents;
            current.OnIpChanged += OnIpChanged;
        }

        private void SetStatusText(string text) {
            contextViewStatus0.Text = text;
        }

        private void UpdateStatus() {
            if (Executor.Current == null) {
                SetStatusStripEmpty();
            }
            SetStatusFromContinuation();
        }

        private void SetStatusStripEmpty() {
            SetStatusText("No Continuation");
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
            UpdateStatus();
        }

        private void UpdateCont(Continuation continuation) {
            Executor.PushContext(continuation);
            UpdateContinuation(continuation);
            OnIpChanged(continuation, 0, 0);
        }

        private void UpdateContinuation(Continuation current) {
            codeView.Items.Clear();
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

            SetStatusFromContinuation();
        }

        private void SetStatusFromContinuation() {
            Continuation current = Executor.Current;
            if (current == null) {
                SetStatusStripEmpty();
                return;
            }

            string opText; 
            if (current.Ip < current.Code.Count) {
                opText = $"[{Registry.ToPiScript(current.Code[current.Ip])}]";
            } else {
                opText = "[At End]";
            }
            SetStatusText($"{current.Name} {current.Ip}/{current.Code.Count} {opText}");
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

        private void UpdateContextStack(Executor executor, List<Continuation> contexts) {
            contextStack.Items.Clear();
            var n = 0;
            foreach (var cont in contexts) {
                AddContinuationToStack(n++, cont);
            }
        }

        private void AddContinuationToStack(int n, Continuation continuation) {
            var row = new ListViewItem(n.ToString());
            var loc = new ListViewItem.ListViewSubItem(row, $"{continuation.Ip}");
            // var code = new ListViewItem.ListViewSubItem(row, Registry.ToPiScript(continuation));
            var code = new ListViewItem.ListViewSubItem(row, continuation.ToString());
            row.SubItems.Add(loc);
            row.SubItems.Add(code);
            contextStack.Items.Add(row);
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
            try {
                exec.Next();
            } catch (Exception ex) {
                var current = exec.Current;
                if (current != null) {
                    MessageBox.Show("Nothing to Continue", "If a tree falls in the woods, does anyone hear it?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show(ex.Message, $"Error executing {current.Code[current.Ip]}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cont == null) {
                return;
            }
            if (cont.Ip > cont.Code.Count) {
                MessageBox.Show("End of Continuation", "At End", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateStatus();
        }

        internal void UpdateView() {
            UpdateContextStack(Executor, Executor.ContextStack);
            UpdateContinuation(Executor.Current);
            UpdateStatus();
        }

        private void runButton_Click(object sender, EventArgs e) {
            MainForm.RunCurrent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            Executor.Clear();
        }
    }
}

