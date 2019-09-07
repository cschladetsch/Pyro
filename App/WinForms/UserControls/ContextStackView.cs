using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms
{
    public partial class ContextStackView : UserControl
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

        public void Clear()
        {
            listView1.Items.Clear();
        }
    }
}
