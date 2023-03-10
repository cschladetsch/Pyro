using Pyro;
using Pyro.Exec;
using System;
using System.Collections.Generic;
using System.Linq;
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
            piPrompt1.Construct(mainForm);
        }

        private void DataStackChanged(Executor executor, Stack<object> dataStack) {
            stackView.Items.Clear();
            var n = 0;
            foreach (var item in dataStack.Reverse()) {
                stackView.Items.Add(MakeStackViewItem(n++, item));
            }

            UpdateStatusText();
        }

        private void UpdateStatusText() {
            toolStripStatusLabel1.Text = $"{stackView.Items.Count} Items";
        }

        //public static void Populate(registry, ListView list, IEnumerable objects) {
        //    list.Clear();
        //    var n = 0;
        //    foreach (var item in objects)
        //        list.Items.Add(MakeStackViewItem(n++, item));
        //}

        private ListViewItem MakeStackViewItem(int n, object item) {
            var row = new ListViewItem(n.ToString());
            AddSubItem(row, MainForm.Registry.ToPiScript(item));
            AddSubItem(row, GetType(item));
            if (item as IReflected != null) {
                AddSubItem(row, (item as IReflected).SelfBase.Id.ToString());
            }
            return row;
        }

        public static string GetType(object item) {
            if (item == null) {
                return "null";
            }
            var type = item.GetType().Name;
            if (type.StartsWith("System.")) {
                type = type.Substring(7);
            }
            return type;
        }

        public static void AddSubItem(ListViewItem row, string text)
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
