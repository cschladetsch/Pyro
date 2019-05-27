using System;
using System.Collections.Generic;
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

        public MainForm()
        {
            InitializeComponent();
            _context = new Context();
            Perform(EOperation.Clear);
            output.Text = "Pyro 0.4a";
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

        private void ExecuteRho()
        {
            TimeTaken(() => _context.ExecRho(rhoText.Text));
        }

        private void TimeTaken(Action action)
        {
            var start = DateTime.Now;
            try
            {
                action();
                var span = DateTime.Now - start;
                var text = $"Took {span.TotalMilliseconds:0.##}ms";
                Console.WriteLine(text);
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
            => TimeTaken(() => Exec.Perform(op));

        private void ExecuteClick(object sender, EventArgs e)
            => ExecuteRho();

        private void StackClearClick(object sender, EventArgs e)
            => Perform(EOperation.Clear);

        private void StackCountClick(object sender, EventArgs e)
            => Perform(EOperation.Depth);

        private void StackOverClick(object sender, EventArgs e)
            => Perform(EOperation.Over);
    }
}

