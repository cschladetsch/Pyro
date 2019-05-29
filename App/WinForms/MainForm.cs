using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using Pyro.Exec;
using Pyro.ExecutionContext;

namespace WinForms
{
    /// <inheritdoc />
    /// <summary>
    /// The main form for the application.
    ///
    /// TODO: Make DataStack redraw Reactive
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly Context _context;
        private Executor Exec => _context.Executor;
        private Stack<object> DataStack => Exec.DataStack;
        private List<object> _last;
        private string piFile, rhoFile;

        public MainForm()
        {
            InitializeComponent();
            _context = new Context();
            Perform(EOperation.Clear);
            output.Text = "Pyro 0.4a";
            mainTabControl.SelectedIndex = 1;
            mainTabControl.SelectedIndexChanged += ChangedTab;

            try
            {
                piInput.Text = LoadFile("pi");
                rhoInput.Text = LoadFile("rho");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Closing += (a,b) =>
            {
                SaveFile("pi", piInput.Text);
                SaveFile("rho", rhoInput.Text);
            };
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

        private void piInput_KeyDown(object sender, KeyEventArgs e)
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
            var ln = piInput.GetLineFromCharIndex(piInput.SelectionStart);
            var ip = piInput.Lines[ln];
            Console.WriteLine(ip);
            Perform(() => _context.ExecPi(ip));
        }

        private void ExecuteRho()
        {
            var script = rhoInput.SelectedText.Length > 0 ? rhoInput.SelectedText : rhoInput.Text;
            Perform(() => _context.ExecRho(script));
        }

        private void Perform(Action action)
        {
            // TODO: CopyStack();

            try
            {
                _context.Reset();
                var start = DateTime.Now;
                action();
                var span = DateTime.Now - start;
                var text = $"Took {span.TotalMilliseconds:0.00}ms";
                toolStripStatusLabel1.Text = text;
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
            AddSubItem(row, _context.Registry.ToText(item));
            return row;
        }

        private static void AddSubItem(ListViewItem row, string text)
            => row.SubItems.Add(new ListViewItem.ListViewSubItem(row, text));

        private void Perform(EOperation op)
            => Perform(() => Exec.Perform(op));

        private void ExecuteClick(object sender, EventArgs e)
            => ExecuteRho();

        private void StackClearClick(object sender, EventArgs e)
            => Perform(EOperation.Clear);

        private void StackCountClick(object sender, EventArgs e)
            => Perform(EOperation.Depth);

        private void StackOverClick(object sender, EventArgs e)
            => Perform(EOperation.Over);

        private void Exit(object sender, EventArgs e)
            => Application.Exit();

        private void ShowAboutBox(object sender, EventArgs e)
            => new AboutBox().ShowDialog();

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

        private void DropClick(object sender, EventArgs e)
        {
            Perform(EOperation.Drop);
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
    }
}

