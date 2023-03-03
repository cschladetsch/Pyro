namespace WinForms.UserControls {
    using System;
    using System.Windows.Forms;

    public partial class PiDebuggerInput
        : UserControlBase
        , IUserControlCommon {
        public PiDebuggerInput() {
            InitializeComponent();
        }

        private void PiDebuggerInput_Load(object sender, EventArgs e) {

        }

        private void PiDebuggerInput_PreviewKeyDown(PreviewKeyDownEventArgs e) {
            Console.WriteLine(e);
            //            if (e.Control && (e.KeyCode == Keys.Enter))
            //            {
            //                var pi = richTextBox1.Lines[richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart)];
            //                try
            //                {
            //                    if (_Main.Context.Translate(pi, out var cont))
            //                    {
            //                        _Exec.PushContext(cont);
            //                        _piDebugger.Restart();
            //                    }
            //                }
            //                catch (Exception exception)
            //                {
            //                    Console.WriteLine(exception);
            //                }
            //            }
        }

        public void Input(PreviewKeyDownEventArgs previewKeyDownEventArgs) {
            PiDebuggerInput_PreviewKeyDown(previewKeyDownEventArgs);
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e) {
            Console.WriteLine(richTextBox1.SelectedText);
        }

        private void RichTextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            Console.WriteLine(e);
        }

        private void RichTextBox1_KeyPress(object sender, KeyPressEventArgs e) {
            Console.WriteLine(e);
        }

        public override void Construct(IMainForm mainForm) {
            throw new NotImplementedException();
        }

        public override void Render() {
            throw new NotImplementedException();
        }
    }
}

