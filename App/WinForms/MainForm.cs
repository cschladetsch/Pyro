using System;
using System.Windows.Forms;

using Pyro.ExecutionContext;

namespace WinForms
{
    /// <inheritdoc />
    /// <summary>
    /// The main form for the application.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly Context _context;

        public MainForm()
        {
            InitializeComponent();

            _context = new Context();
        }

        private void RhoTextKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                {
                    if (e.Control)
                        ExecuteRho();
                    e.Handled = true;
                    break;
                }
            }
        }

        private void ExecuteRho()
        {
            _context.ExecRho(rhoText.Text);
            output.Text = _context.Error;

            UpdateStackView();
        }

        private void UpdateStackView()
        {
            stackView.Items.Clear();
            var n = 0;
            foreach (var item in _context.Executor.DataStack)
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

        private void ExecuteClick(object sender, EventArgs e)
            => ExecuteRho();
    }
}

