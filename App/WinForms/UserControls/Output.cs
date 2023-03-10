namespace WinForms.UserControls {
    using System.Drawing;
    using System.Windows.Forms;

    public partial class Output
        : UserControlBase
        , IUserControlCommon {

        public RichTextBox TextBox => richTextBox1;

        public new string Text { get => richTextBox1.Text; set => richTextBox1.Text = value; }

        public void Append(string text) {
            TextBox.AppendText(text);
        }

        public void Append(string text, Color color) {
            var box = richTextBox1;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor; ;
        }

        public Output() {
            InitializeComponent();
        }

        public override void Construct(IMainForm mainForm) {
            base.Construct(mainForm);
            Pyro.Exec.Executor.OnDebugTrace += (text) => Append(text);
        }

    }
}
