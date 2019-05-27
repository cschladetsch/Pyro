using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pyro.ExecutionContext;

namespace WinForms
{
    public partial class MainForm : Form
    {
        private readonly Context _context;

        public MainForm()
        {
            InitializeComponent();

            _context = new Context();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rhoText_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
            case Keys.Enter:
            {
                if (e.Control)
                    ExecuteRho();
                break;
            }

            case Keys.Tab:
                if (e.Shift == true)
                    UnIndent();
                else
                    Indent();
                e.Handled = true;
                break;
            }
        }

        private void UnIndent()
        {
        }

        private void Indent()
        {
        }

        private void ExecuteRho()
        {
            var script = rhoText.Text;
            _context.ExecRho(script);
            output.Text = _context.Error;

            UpdateStackView();
        }

        private void UpdateStackView()
        {
            stackView.Items.Clear();
            var n = 0;
            foreach (var item in _context.Executor.DataStack)
            {
                var row = MakeStackViewItem(n++, item);
                stackView.Items.Add(row);
            }
        }

        private ListViewItem MakeStackViewItem(int n, object item)
        {
            var row = new ListViewItem();
            var num = new ListViewItem.ListViewSubItem(row, n.ToString());
            var text = new ListViewItem.ListViewSubItem(row, _context.Registry.ToText(item));
            row.SubItems.Add(num);
            row.SubItems.Add(text);
            return row;
        }

        private void execute_Click(object sender, EventArgs e)
        {
            ExecuteRho();
        }
    }
}

