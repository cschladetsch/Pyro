namespace WinForms {
    using Pyro;
    using Pyro.Exec;
    using Pyro.ExecutionContext;
    using Pyro.Network;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using WinForms.UserControls;
    using static Pyro.Create;


    /// <summary>
    /// The main form for the application.
    /// </summary>
    public partial class MainForm
        : Form
        , IMainForm {
        public int ListenPort = 7777;

        public Executor Executor => _context.Executor;

        public IRegistry Registry => _context.Registry;

        public Stack<object> DataStack => Executor.DataStack;

        private List<object> _last;
        private bool _local = true;
        private bool PiSelected => mainTabControl.SelectedIndex == 0;

        internal ExecutionContext Context => _context;
        internal RichTextBox rhoInput => rhoEditorControl1.RichTextBox;

        int IMainForm.ListenPort => throw new NotImplementedException();

        public ContextStackView ContextView => contextStackView1;

        private readonly IPeer _peer;
        private readonly ExecutionContext _context;
        public MainForm() {
            InitializeComponent();

            _context = new ExecutionContext();
            _peer = Pyro.Network.Create.NewPeer(ListenPort);
            _peer.OnConnected += Connected;
            _peer.OnReceivedResponse += Received;

            Pyro.Network.RegisterTypes.Register(_context.Registry);

            // Clear the data stack from any design-time junk.
            Perform(EOperation.Clear);

            output1.Text = Pyro.AppCommon.AppCommonBase.GetVersion();
            mainTabControl.SelectedIndex = 1;
            mainTabControl.SelectedIndexChanged += ChangedTab;

            piInput.TextChanged += PiInputOnTextChanged;
            rhoInput.TextChanged += RhoInputOnTextChanged;
            rhoInput.KeyDown += RhoTextKeyDown;

            LoadPrevious();

            FormClosing += (a, b) => {
                SaveFile("pi", piInput.Text);
                SaveFile("rho", rhoInput.Text);
                _peer?.Stop();
            };

            UpdatePiContext();
            ColorisePi();
            ColoriseRho();
            AddBuiltinMethods();
            ConnectUserControls();
            
            Executor.Rethrows = true;
        }

        private void AddBuiltinMethods() {
            Executor.Scope["TimeNow"] = Function(() => DateTime.Now);
            Executor.Scope["PrintSpan"] = Function<TimeSpan>(d => Print(d.ToString()));
            Executor.Scope["PrintTime"] = Function<DateTime>(d => Print(d.ToString()));
            Executor.Scope["print"] = Function<object>(d => Print(d.ToString()));
        }

        private void ConnectUserControls() {
            dataStackView1.Construct(this);
            contextStackView1.Construct(this);
            output1.Construct(this);
        }

        private void Print(object obj) {
            if (obj == null)
                return;

            Console.WriteLine(obj);
            output1.Text += "\n" + obj.ToString();
        }

        private void Received(IClient client, string text) {
            if (InvokeRequired) {
                Invoke(new MessageHandler(Received), client, text);
                return;
            }

            Console.WriteLine($"Recv: {text}");
            if (_context.Translate(text, out var cont)) {
                try {
                    Executor.Continue(cont.Code[0] as Continuation);
                } catch (Exception e) {
                    OutputException(e);
                }
            }
        }

        private void LoadPrevious() {
            piInput.Text = LoadFile("pi");
            rhoInput.Text = LoadFile("rho");
        }

        private void ChangedTab(object sender, EventArgs e) {
            if (mainTabControl.SelectedIndex != 0)
                return;

            UpdatePiContext();
        }

        private void UpdatePiContext() {
            // TODO: Add concept of a `tree`
            piStatus.Text = "/home λ";
        }

        private void RhoTextKeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Enter: {
                        if (e.Control) {
                            ExecuteRho();
                            e.Handled = true;
                        }
                        if (e.Alt & e.Control) {
                            UpdateContextView();
                            e.Handled = true;
                        }
                        break;
                    }
            }
        }

        private void UpdateContextView() {
            contextStackView1.UpdateView();
        }

        private void ChangeTab(bool ctrl, bool shift) {
            // TODO: this is stupid
            int Next(int cur, int tot) => (cur + 1) % tot;
            int Prev(int cur, int tot) => (cur + tot - 1) % tot;
            NextTab(ctrl ? (Func<int, int, int>)Prev : Next);
        }

        private void NextTab(Func<int, int, int> act) {
            var numTabs = mainTabControl.TabPages.Count;
            var curTab = mainTabControl.SelectedIndex;
            mainTabControl.SelectedIndex = act(curTab, numTabs);
        }

        private void PiInputKeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Enter: {
                        if (e.Control) {
                            ExecutePi();
                            e.Handled = true;
                        }
                        break;
                    }
            }
        }

        private void ExecutePi() {
            try {
                var pi = piInput.Lines[piInput.GetLineFromCharIndex(piInput.SelectionStart)];
                if (_local)
                    Perform(() => _context.ExecPi(pi));
                else
                    Perform(() => _peer.Execute(pi));
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void ExecuteRho() {
            try {
                var script = rhoInput.SelectedText.Length > 0 ? rhoInput.SelectedText : rhoInput.Text;
                if (_local)
                    Perform(() => _context.ExecRho(script));
                else
                    Perform(() => _peer.Execute(script));
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void Perform(Action action) {
            // TODO: CopyStack(); to answer `get last stack` request.
            try {
                _context.Reset();
                var start = DateTime.Now;
                action();
                var span = DateTime.Now - start;
                toolStripStatusLabel1.Text = $"Took {span.TotalMilliseconds:0.00}ms";
                if (!string.IsNullOrEmpty(_context.Error))
                    output1.Append("\n" + _context.Error);
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void OutputException(Exception e) {
            var text = $"{e.Message} ({_context.Error})";
            if (e.Message == _context.Error) {
                text = $"{e.Message}";
            }
            output1.Append(text, Color.Red);
            Console.WriteLine(e);
            MessageBox.Show(text, "Pyro Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CopyStack() {
            try {
                _last = new List<object>();
                foreach (var obj in Executor.DataStack) {
                    _last.Add(_context.Registry.Duplicate(obj)); // TODO: copy-on-write duplicates
                }
            } catch (Exception e) {
                OutputException(e);
            }
        }

        public void Perform(EOperation op) {
            try {
                if (_local)
                    Perform(() => Executor.Perform(op));
                else
                    _peer.Execute(_context.Registry.ToPiScript(op));
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void SaveAsFile(object sender, EventArgs e) {
            var isPi = PiSelected;
            var save = isPi ? savePiDialog : saveRhoDialog;
            if (save.ShowDialog() == DialogResult.OK)
                File.WriteAllText(save.FileName, isPi ? piInput.Text : rhoInput.Text);
        }

        private void GotoSourceClick(object sender, EventArgs e)
            => System.Diagnostics.Process.Start(@"https://www.github.com/cschladetsch/pyro");

        private void ShowAboutBox(object sender, EventArgs e)
            => new AboutBox().ShowDialog();

        private void Exit(object sender, EventArgs e)
            => Application.Exit();

        private static string LoadFile(string name)
            => File.ReadAllText(TmpFile(name));

        private static void SaveFile(string name, string contents)
            => File.WriteAllText(TmpFile(name), contents);

        private static string TmpFile(string name)
            => Path.Combine(GetFolderPath(), $"last.{name}");

        private static string GetFolderPath()
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private void SaveFile(object sender, EventArgs e)
            => throw new NotImplementedException();

        private void NetworkConnect(object sender, EventArgs e)
            => new NetworkConnect(_peer).Show();

        private void PiInputOnTextChanged(object sender, EventArgs e)
            => ColorisePi();

        private void RhoInputOnTextChanged(object sender, EventArgs e)
            => ColoriseRho();

        private static void Connected(IPeer peer, IClient client) {
            Console.WriteLine($"Connected: {peer} {client}");
        }

        private void loadRhoToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void rhoInput_TextChanged(object sender, EventArgs e) {
        }

        private void piDebugger1_Load(object sender, EventArgs e) {
        }

        private void rhoEditorControl1_Load(object sender, EventArgs e) {
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleControlVisibility(contextStackView1);
        }

        private void treeToolStripMenuItem_Click(object sender, EventArgs e) {
            //ToggleControlVisibility(treeView1);
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleControlVisibility(output1);
        }
        private void stackToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleControlVisibility(dataStackView1);
        }

        private void ToggleControlVisibility(UserControl control) {
            if (control.Visible) {
                control.Visible = false;
            } else {
                control.Visible = true;
            }
        }

        private void decompile_Click(object sender, EventArgs e) {
            var cont = GetContinuation();
            if (cont == null) {
                return;
            }
            Executor.PushContext(cont);
            UpdateContextView();
        }

        private void run_Click(object sender, EventArgs e) {
            ExecuteRho();
        }

        Continuation GetContinuation() {
            var isRho = mainTabControl.SelectedIndex == 1;
            var text = isRho ? rhoEditorControl1.RichTextBox.Text : piInput.Text;
            _context.Language = Pyro.Language.ELanguage.Rho;
            if (!_context.Translate(text, out Continuation cont)) {
                MessageBox.Show(_context.Error, "Failed to Translate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return cont;
        }

        private void toPi_Click(object sender, EventArgs e) {
            var cont = GetContinuation();
            if (cont == null) {
                return;
            }
            piInput.Text = Registry.ToPiScript(cont);
            ColorisePi();
            mainTabControl.SelectedIndex = 0;
        }
    }
}

