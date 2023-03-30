using System.Drawing;
using System.Windows.Forms;
using Pyro.Exec;

namespace WinForms.UserControls {
    public partial class Output
        : UserControlBase
            , IUserControlCommon {
        public Output() {
            InitializeComponent();
        }

        public RichTextBox TextBox { get; private set; }

        public new string Text {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        public override void Construct(IMainForm mainForm) {
            base.Construct(mainForm);
            Executor.OnDebugTrace += text => Append(text);
        }

        public void Append(string text) {
            TextBox.AppendText(text);
        }

        public void Append(string text, Color color) {
            var box = TextBox;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            ;
        }
    }
}