using WinForms.UserControls;

namespace WinForms
{
    using Pyro;
    using Pyro.Exec;
    using Pyro.ExecutionContext;
    using Pyro.Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;
    using static Pyro.Create;

    /// <summary>
    /// The main form for the application.
    /// </summary>
    public partial class MainForm
        : Form
    {
        public int ListenPort = 7777;

        private readonly IPeer _peer;
        private readonly Context _context;
        private Executor Exec => _context.Executor;
        private IRegistry Reg => _context.Registry;
        private Stack<object> DataStack => Exec.DataStack;
        private List<object> _last;
        private bool _local = true;
        private bool PiSelected => mainTabControl.SelectedIndex == 0;

        internal RichTextBox Output => output;
        internal Context Context => _context;
        internal PiDebugger PiDebugger => piDebugger1;
        internal ContextStackView ContextStackView => contextStackView6;

        public MainForm()
        {
            InitializeComponent();

            _context = new Context();
            _peer = Pyro.Network.Create.NewPeer(ListenPort);
            _peer.OnConnected += Connected;
            _peer.OnReceivedResponse += Received;

            Pyro.Network.RegisterTypes.Register(_context.Registry);

            // Clear the data stack from any design-time junk.
            Perform(EOperation.Clear);

            output.Text = GetVersion();
            mainTabControl.SelectedIndex = 2;
            mainTabControl.SelectedIndexChanged += ChangedTab;
            piInput.TextChanged += PiInputOnTextChanged;
            rhoInput.TextChanged += RhoInputOnTextChanged;

            LoadPrevious();

            Closing += (a, b) =>
            {
                SaveFile("pi", piInput.Text);
                SaveFile("rho", rhoInput.Text);
                _peer?.Stop();
            };

            UpdatePiContext();
            ColorisePi();
            ColoriseRho();
            Exec.Scope["TimeNow"] = Function(() => DateTime.Now);
            Exec.Scope["PrintSpan"] = Function<TimeSpan>(d => Print(d.ToString()));
            Exec.Scope["PrintTime"] = Function<DateTime>(d => Print(d.ToString()));
            Exec.Scope["print"] = Function<object>(d => Print(d.ToString()));

            Exec.Rethrows = true;

            SetupPiDebug();

            var timer = new System.Windows.Forms.Timer { Interval = 10 };
            timer.Tick += (sender, args) => Exec.Next();
            timer.Start();
        }

        private void SetupPiDebug()
        {
            piDebugger1.Construct(this);
            piInputDebugger1.Construct(this);
            dataStackView2.Construct(this);
            contextStackView6.Construct(this);

            ClearDebug();
        }

        private void ClearDebug()
        {
            piDebugger1.Clear();
            dataStackView2.Clear();
            contextStackView6.Clear();
        }

        private void Print(object obj)
        {
            if (obj == null)
                return;

            Console.WriteLine(obj);
            output.Text += "\n" + obj.ToString();
        }

        private void Received(IServer server, IClient client, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MessageHandler(Received), server, client, text);
                return;
            }

            Console.WriteLine($"Recv: {text}");
            if (_context.Translate(text, out var cont))
            {
                try
                {
                    Exec.Continue(cont.Code[0] as Continuation);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    output.Text += $@"Exception: {e.Message}";
                }
            }

            UpdateStackView();
        }

        private void LoadPrevious()
        {
            try
            {
                piInput.Text = LoadFile("pi");
                rhoInput.Text = LoadFile("rho");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                output.Text += $"Exception: {e.Message}";
            }
        }

        private void ChangedTab(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedIndex != 0)
                return;

            UpdatePiContext();
        }

        private void UpdatePiContext()
        {
            // TODO: Add concept of a `tree`
            piStatus.Text = "/home λ";
        }

        private void RhoTextKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //case Keys.Tab:
                //{
                //    ChangeTab(e.Control, e.Shift);
                //    break;
                //}

                case Keys.Enter:
                {
                    if (e.Control)
                    {
                        ExecuteRho();
                        e.Handled = true;
                    }

                    break;
                }
            }
        }

        private void ChangeTab(bool ctrl, bool shift)
        {
            // TODO: this is stupid
            int Next(int cur, int tot) => (cur + 1) % tot;
            int Prev(int cur, int tot) => (cur + tot - 1) % tot;
            NextTab(ctrl ? (Func<int, int, int>)Prev : Next);
        }

        private void NextTab(Func<int, int, int> act)
        {
            var numTabs = mainTabControl.TabPages.Count;
            var curTab = mainTabControl.SelectedIndex;
            mainTabControl.SelectedIndex = act(curTab, numTabs);
        }

        private void PiInputKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
            case Keys.Enter:
                {
                    int n;
                    if (e.Control)
                    {
                        ExecutePi();
                        e.Handled = true;
                    }

                    break;
                }
            }
        }

        private void ExecutePi()
        {
            var pi = piInput.Lines[piInput.GetLineFromCharIndex(piInput.SelectionStart)];
            if (_local)
                Perform(() => _context.ExecPi(pi));
            else
                Perform(() => _peer.Execute(pi));
        }

        private void ExecuteRho()
        {
            var script = rhoInput.SelectedText.Length > 0 ? rhoInput.SelectedText : rhoInput.Text;
            if (_local)
                Perform(() => _context.ExecRho(script));
        }

        private void Perform(Action action)
        {
            // TODO: CopyStack(); to answer `get last stack` request.
            try
            {
                _context.Reset();
                var start = DateTime.Now;
                action();
                var span = DateTime.Now - start;
                toolStripStatusLabel1.Text = $"Took {span.TotalMilliseconds:0.00}ms";
                if (!string.IsNullOrEmpty(_context.Error))
                    output.Text += "\n" + _context.Error;
                UpdateStackView();
            }
            catch (Exception e)
            {
                output.Text += $"Exception: {e.Message} ({_context.Error})";
                Console.WriteLine(e);
            }
        }

        private void CopyStack()
        {
            try
            {
                _last = new List<object>();
                foreach (var obj in Exec.DataStack)
                {
                    _last.Add(_context.Registry.Duplicate(obj)); // TODO: copy-on-write duplicates
                }
            }
            catch (Exception e)
            {
                output.Text = $"Exception: {e.Message} ({_context.Error})";
                Console.WriteLine(e);
            }
        }

        private void UpdateStackView()
        {
            stackView.Items.Clear();
            var n = 0;
            foreach (var item in DataStack)
                stackView.Items.Add(MakeStackViewItem(n++, item));
        }

        private ListViewItem MakeStackViewItem(int n, object item)
        {
            var row = new ListViewItem();
            AddSubItem(row, n.ToString());
            AddSubItem(row, _context.Registry.ToPiScript(item));
            return row;
        }

        private void Perform(EOperation op)
        {
            if (_local)
                Perform(() => Exec.Perform(op));
            else
                _peer.Execute(_context.Registry.ToPiScript(op));
        }

        private void SaveAsFile(object sender, EventArgs e)
        {
            var isPi = PiSelected;
            var save = isPi ? savePiDialog : saveRhoDialog;
            if (save.ShowDialog() == DialogResult.OK)
                File.WriteAllText(save.FileName, isPi ? piInput.Text : rhoInput.Text);
        }

        private static void AddSubItem(ListViewItem row, string text)
            => row.SubItems.Add(new ListViewItem.ListViewSubItem(row, text));

        private void StackClearClick(object sender, EventArgs e)
            => Perform(EOperation.Clear);

        private void StackCountClick(object sender, EventArgs e)
            => Perform(EOperation.Depth);

        private void StackOverClick(object sender, EventArgs e)
            => Perform(EOperation.Over);

        private void StackDupClick(object sender, EventArgs e)
            => Perform(EOperation.Dup);

        private void StackSwapClick(object sender, EventArgs e)
            => Perform(EOperation.Swap);

        private void StackDropClick(object sender, EventArgs e)
            => Perform(EOperation.Drop);

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

        private static void Connected(IPeer peer, IClient client)
            => Console.WriteLine($"Connected: {peer} {client}");

        private void PiDebuggerInputPreview(object sender, PreviewKeyDownEventArgs e)
        {
            var pi = piInput.Lines[piInput.GetLineFromCharIndex(piInput.SelectionStart)];
            piDebugger1.Input(pi);
        }
    }
}

