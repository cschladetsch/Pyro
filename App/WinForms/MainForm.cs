using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Pyro;
using Pyro.Exec;
using Pyro.ExecutionContext;
using Pyro.Language;
using Pyro.Language.Lexer;
using Pyro.Network;
using Pyro.Network.Impl;

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

            output.Text = Pyro.AppCommon.AppCommonBase.GetVersion();
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

        private void PiInputOnTextChanged(object sender, EventArgs e)
        {
            ColorisePi();
        }

        private void Connected(IPeer peer, IClient client)
        {
            Console.WriteLine($"Connected: {peer} {client}");
        }

        private void Received(IServer server, IClient client, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new ReceivedResponseHandler(Received), server, client, text);
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

        private static string LoadFile(string name)
            => File.ReadAllText(TmpFile(name));

        private static void SaveFile(string name, string contents)
            => File.WriteAllText(TmpFile(name), contents);

        private static string TmpFile(string name)
            => Path.Combine(GetFolderPath(), $"last.{name}");

        private static string GetFolderPath()
            => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private void ChangedTab(object sender, EventArgs e)
        {
            var isPi = mainTabControl.SelectedIndex == 0;
            if (!isPi)
                return;
            UpdatePiContext();
        }

        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        private void UpdatePiContext()
        {
            // TODO: Add concept of a `tree`
            piStatus.Text = "/home λ";
            //var stack = _context.Executor.ContextStack;
            //if (stack.Count == 0)
            //    return;
            //var top = stack.Peek();
            //piStatus.Text = $"{
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

                    // no re-color
                    return;
                }

                // don't need to re-color for these keys
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Home:
                case Keys.End:
                case Keys.Insert:
                case Keys.Control:
                case Keys.Delete:
                    e.Handled = false;
                    return;
            }
        }

        private void ExecutePi()
        {
            var ln = piInput.GetLineFromCharIndex(piInput.SelectionStart);
            var ip = piInput.Lines[ln];
            Console.WriteLine(ip);
            if (_local)
                Perform(() => _context.ExecPi(ip));
            else
                Perform(() => _peer.Execute(ip));
        }

        private void ExecuteRho()
        {
            var script = rhoInput.SelectedText.Length > 0 ? rhoInput.SelectedText : rhoInput.Text;
            if (_local)
                Perform(() => _context.ExecRho(script));
        }

        private void Perform(Action action)
        {
            // TODO: CopyStack(); for `get last stack` request.

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

        private static void AddSubItem(ListViewItem row, string text)
            => row.SubItems.Add(new ListViewItem.ListViewSubItem(row, text));

        private void Perform(EOperation op)
        {
            if (_local)
                Perform(() => Exec.Perform(op));
            else
            {
                // ....
            }
        }

        private void ExecuteClick(object sender, EventArgs e)
            => ExecuteRho();

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

        private void piConsole_Click(object sender, EventArgs e)
        {
        }

        private void stackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //stackView.
            //output.Dock = DockStyle.Fill;
            //stackView.Enabled = !stackView.Enabled;
            //if (stackView.Is)
        }

        private bool PiSelected => mainTabControl.SelectedIndex == 0;

        private void SaveAsFile(object sender, EventArgs e)
        {
            var isPi = PiSelected;
            var save = isPi ? savePiDialog : saveRhoDialog;
            if (save.ShowDialog() == DialogResult.OK)
                File.WriteAllText(save.FileName, isPi ? piInput.Text : rhoInput.Text);
        }

        private void SaveFile(object sender, EventArgs e)
        {
        }

        private void _NetworkConnect(object sender, EventArgs e)
        {
            var dlg = new NetworkConnect(_peer);
            dlg.Show();
        }
    }
}

