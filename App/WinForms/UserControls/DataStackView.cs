using Pyro.Exec;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms.UserControls {
    public partial class DataStackView
        : UserControlBase
        , IUserControlCommon {
        public DataStackView() {
            InitializeComponent();
        }

        public override void Construct(IMainForm mainForm) {
            base.Construct(mainForm);
            MainForm.Executor.OnDataStackChanged += DataStackChanged;
        }

        private void DataStackChanged(Executor executor, Stack<object> dataStack) {
            stackView.Items.Clear();
            var n = 0;
            foreach (var item in dataStack)
                stackView.Items.Add(MakeStackViewItem(n++, item));
        }

        private ListViewItem MakeStackViewItem(int n, object item) {
            var row = new ListViewItem(n.ToString());
            AddSubItem(row, MainForm.Registry.ToPiScript(item));
            AddSubItem(row, GetType(item));
            AddSubItem(row, "#?");
            return row;
        }

        private string GetType(object item) {
            if (item == null) {
                return "null";
            }
            var type = item.GetType().Name;
            if (type.StartsWith("System.")) {
                type = type.Substring(7);
            }
            return type;
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

    }
}
