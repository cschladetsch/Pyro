using System.Windows.Forms;

namespace WinForms.UserControls {
    public partial class RhoEditorControl : UserControl {
        public RichTextBox RichTextBox { get; private set; }

        public RhoEditorControl() {
            InitializeComponent();

            RichTextBox.Multiline = true;
            RichTextBox.AcceptsTab = true;
        }
    }
}
