using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Pyro;
using Pyro.AppCommon;
using Pyro.Exec;
using Pyro.ExecutionContext;
using Pyro.Language;
using Pyro.Network;
using Pyro.Network.Impl;
using WinForms.UserControls;
using Process = System.Diagnostics.Process;
using Timer = System.Windows.Forms.Timer;

namespace WinForms {
    using static Create;


    /// <summary>
    ///     The main form for the application.
    /// </summary>
    public partial class MainForm
        : Form
            , IMainForm {
        private readonly ExecutionContext _context;
        private readonly IDomain _netDomain = new Domain();
        private readonly int _saveTimerIntervalMills = 500;

        private List<object> _last;
        private readonly bool _localProcess = true;
        private IPeer _peer;
        private Timer _timer;

        public MainForm() {
            InitializeComponent();

            _context = new ExecutionContext();

            Perform(EOperation.Clear);
            AddEventHandlers();
            LoadPrevious();
            ColorisePi();
            ColoriseRho();
            AddBuiltinMethods();
            ConnectUserControls();
            AddClosingEvent();
            SaveAllRegularly();
            SetupNetwork();

            Executor.Rethrows = true;
            Executor.Verbosity = 20;
            editor.Language = ELanguage.Pi;
            Decompile();
            output.Text = ConsoleAppCommonBase.GetVersion() + '\n';
        }

        public Stack<object> DataStack => Executor.DataStack;
        private RichTextBox _piInput => editor.GetLanguageText(ELanguage.Pi);
        private RichTextBox _rhoInput => editor.GetLanguageText(ELanguage.Rho);
        private RichTextBox _tauInput => editor.GetLanguageText(ELanguage.Tau);
        public int ListenPort { get; } = 7777;

        public Executor Executor => _context.Executor;

        public IRegistry Registry => _context.Registry;

        public ContextStackView ContextView { get; private set; }


        public void Run(string text, ELanguage language) {
            try {
                if (_localProcess) {
                    Perform(() => _context.Exec(language, text));
                    return;
                }

                var script = language == ELanguage.Pi ? text : Registry.ToPiScript(text);
                Perform(() => _peer.Execute(script));
            } catch (Exception e) {
                OutputException(e);
            }
        }

        public void Perform(EOperation op) {
            try {
                if (_localProcess) {
                    Perform(() => Executor.Perform(op));
                }
                else {
                    _peer.Execute(_context.Registry.ToPiScript(op));
                }
            } catch (Exception e) {
                OutputException(e);
            }
        }

        public void Decompile() {
            var cont = GetContinuation();
            if (cont == null) {
                return;
            }

            Executor.PushContext(cont);
            UpdateContextView();
        }

        public void RunCurrent() {
            try {
                Executor.Continue();
            } catch (Exception e) {
                OutputException(e);
            }
        }

        public void ConvertToPi() {
            var cont = GetContinuation();
            if (cont == null) {
                return;
            }

            _piInput.Text = Registry.ToPiScript(cont) + " &";
            ColorisePi();
        }

        private void SaveAllRegularly() {
            _timer = new Timer();
            _timer.Interval = _saveTimerIntervalMills; // milliseconds
            _timer.Tick += SaveAll;
        }

        private void SaveAll(object sender, EventArgs eventArgs) {
            SaveFiles();
        }

        private void AddClosingEvent() {
            FormClosing += (a, b) =>
            {
                SaveFiles();
                _peer?.Stop();
            };
        }

        private void SaveFiles() {
            SaveFile("pi", _piInput.Text);
            SaveFile("rho", _rhoInput.Text);
            SaveFile("tau", _tauInput.Text);
        }

        private void AddEventHandlers() {
            _piInput.TextChanged += PiInputOnTextChanged;
            _rhoInput.TextChanged += RhoInputOnTextChanged;
            _rhoInput.KeyDown += RhoTextKeyDown;
        }

        private void SetupNetwork() {
            _peer = Pyro.Network.Factory.NewPeer(_netDomain, ListenPort);
            _peer.OnConnected += Connected;
            _peer.OnReceivedResponse += Received;

            Pyro.Network.Factory.RegisterTypes(_context.Registry);
        }

        private void AddBuiltinMethods() {
            Executor.Scope["TimeNow"] = Function(() => DateTime.Now);
            Executor.Scope["PrintSpan"] = Function<TimeSpan>(d => Print(d.ToString()));
            Executor.Scope["PrintTime"] = Function<DateTime>(d => Print(d.ToString()));
            Executor.Scope["print"] = Function<object>(d => Print(d.ToString()));
        }

        private void ConnectUserControls() {
            dataStack.Construct(this);
            ContextView.Construct(this);
            output.Construct(this);
            editor.Construct(this);
        }

        private void Print(object obj) {
            if (obj == null) {
                return;
            }

            Console.WriteLine(obj);
            output.Text += "\n" + obj;
        }

        private void Received(IPeer self, IClient client, string text) {
            if (InvokeRequired) {
                Invoke(new MessageHandler(Received), client, text);
                return;
            }

            Console.WriteLine($"Recv: {text}");
            if (!_context.Translate(text, out var cont)) {
                return;
            }

            try {
                Executor.Continue(cont.Code[0] as Continuation);
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void LoadPrevious() {
            _piInput.Text = LoadFile("pi");
            _rhoInput.Text = LoadFile("rho");
        }

        private void RhoTextKeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Enter: {
                    if (e.Control) {
                        RunCurrent();
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
            ContextView.UpdateView();
        }


        private void Perform(Action action) {
            // TODO: CopyStack(); to answer `get last stack` request.
            try {
                _context.Reset();
                var start = DateTime.Now;
                action();
                var span = DateTime.Now - start;
                toolStripMenuItem1.Text = $"Took {span.TotalMilliseconds:0.00}ms (with UI updates)";
                if (!string.IsNullOrEmpty(_context.Error)) {
                    output.Append("\n" + _context.Error);
                }
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void OutputException(Exception e) {
            var text = $"{e.Message} ({_context.Error})";
            if (e.Message == _context.Error) {
                text = $"{e.Message}";
            }

            output.Append(text + '\n', Color.Red);
            Console.WriteLine(e);
            MessageBox.Show(text, "Pyro Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CopyStack() {
            try {
                _last = new List<object>();
                foreach (var obj in
                    Executor.DataStack) _last.Add(_context.Registry.Duplicate(obj)); // TODO: copy-on-write duplicates
            } catch (Exception e) {
                OutputException(e);
            }
        }

        private void SaveAsFile(object sender, EventArgs e) {
            var isPi = editor.Language == ELanguage.Pi;
            var save = isPi ? savePiDialog : saveRhoDialog;
            if (save.ShowDialog() == DialogResult.OK) {
                File.WriteAllText(save.FileName, isPi ? _piInput.Text : _rhoInput.Text);
            }
        }

        private void GotoSourceClick(object sender, EventArgs e) {
            Process.Start(@"https://www.github.com/cschladetsch/pyro");
        }

        private void ShowAboutBox(object sender, EventArgs e) {
            new AboutBox().ShowDialog();
        }

        private void Exit(object sender, EventArgs e) {
            Application.Exit();
        }

        private static string LoadFile(string name) {
            return File.ReadAllText(TmpFile(name));
        }

        private static void SaveFile(string name, string contents) {
            File.WriteAllText(TmpFile(name), contents);
        }

        private static string TmpFile(string name) {
            return Path.Combine(GetFolderPath(), $"last.{name}");
        }

        private static string GetFolderPath() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        private void SaveFile(object sender, EventArgs e) {
            throw new NotImplementedException();
        }

        private void NetworkConnect(object sender, EventArgs e) {
            new NetworkConnect(_peer).Show();
        }

        private void PiInputOnTextChanged(object sender, EventArgs e) {
            ColorisePi();
        }

        private void RhoInputOnTextChanged(object sender, EventArgs e) {
            ColoriseRho();
        }

        private static void Connected(IPeer peer, IClient client) {
            Console.WriteLine($"Connected: {peer} {client}");
        }

        private void loadRhoToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleControlVisibility(ContextView);
        }

        private void treeToolStripMenuItem_Click(object sender, EventArgs e) {
            //ToggleControlVisibility(treeView1);
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleControlVisibility(output);
        }

        private void stackToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleControlVisibility(dataStack);
        }

        private void ToggleControlVisibility(UserControl control) {
            control.Visible = !control.Visible;
        }

        private Continuation GetContinuation() {
            _context.Language = editor.Language;
            if (_context.Translate(editor.RichTextBox.Text, out var cont)) {
                return cont;
            }

            MessageBox.Show(_context.Error, $"Failed to Translate {editor.Language}", MessageBoxButtons.OK
                , MessageBoxIcon.Information);
            return null;
        }
    }
}