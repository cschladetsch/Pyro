using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Pyro;
using Pyro.Exec;
using Pyro.ExecutionContext;
using Pyro.Network;

namespace WinForms
{
    /// <summary>
    /// The main form for the application.
    ///
    /// TODO: Make DataStack redraw Reactive
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

        public MainForm()
        {
            InitializeComponent();

            _context = new Context();
            Pyro.Network.RegisterTypes.Register(_context.Registry);

            //_peer = Create.NewPeer(ListenPort);
            //_peer.OnConnected += Connected;
            //_peer.OnReceivedResponse += Received;
            //if (!_peer.StartSelfHosting())
            //{
            //    Console.WriteLine(_peer.Error);
            //    _local = true;
            //}

            // clear the data stack from any design-time junk.
            Perform(EOperation.Clear);

            output.Text = GetVersion();
            mainTabControl.SelectedIndex = 0;
            mainTabControl.SelectedIndexChanged += ChangedTab;

            LoadPrevious();

            Closing += (a, b) =>
            {
                SaveFile("pi", piInput.Text);
                SaveFile("rho", rhoInput.Text);
                _peer?.Stop();
            };

            UpdatePiContext();
            ColorisePi();

            piInput.TextChanged += PiInputOnTextChanged;
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
                    output.Text = $"Exception: {e.Message}";
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
                Console.WriteLine(e);
            }
        }

        private void ChangedTab(object sender, EventArgs e)
        {
            var isPi = mainTabControl.SelectedIndex == 0;
            if (!isPi)
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
            NextTab(ctrl ? (Func<int, int, int>) Prev : Next);
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
                output.Text = _context.Error;
                UpdateStackView();
            }
            catch (Exception e)
            {
                output.Text = $"Exception: {e.Message} ({_context.Error})";
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
                // TODO
                ;
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

        private void Connected(IPeer peer, IClient client)
            => Console.WriteLine($"Connected: {peer} {client}");
    }
}


