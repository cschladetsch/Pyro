namespace WinForms
{
    using System;
    using System.Windows.Forms;
    using UserControls;
    using Pyro.Exec;

    public partial class ContextStackView
        : UserControlBase
    {
        public ContextStackView()
        {
            InitializeComponent();
        }

        private void ContextStackView_Load(object sender, EventArgs e)
        {
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        internal override void Construct(MainForm mainForm)
        {
            _Main = mainForm;
        }

        public override void Clear()
        {
            listView1.Items.Clear();
        }

        internal void Show(Continuation cont)
        {
            var n = 0;
            foreach (var item in cont.Code)
            {
                AddItem(n++, item);
            }
        }

        private void AddItem(int n, object obj)
        {
            var item = new ListViewItem();
            var subs = new ListViewItem.ListViewSubItem[3];
            if (obj == null)
            {
                subs[0] = NewSubItem(item, n, "null");
                return;
            }

            subs[1] = NewSubItem(item, 1, _Main.Context.Registry.ToPiScript(obj));
            subs[2] = NewSubItem(item, 2, obj.GetType().ToString());
            listView1.Items.Add(item);
        }

        private ListViewItem.ListViewSubItem NewSubItem(ListViewItem item, int n, string text)
        {
            return new ListViewItem.ListViewSubItem(item, text);
        }
    }
}

