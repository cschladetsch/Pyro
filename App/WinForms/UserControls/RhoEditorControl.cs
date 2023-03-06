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
    public partial class RhoEditorControl : UserControl {
        public RichTextBox RichTextBox => richTextBox1;

        public RhoEditorControl() {
            InitializeComponent();

            richTextBox1.Multiline = true;
            richTextBox1.AcceptsTab = true;
        }
    }
}
