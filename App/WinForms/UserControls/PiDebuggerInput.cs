namespace WinForms.UserControls
{
    using System;
    using System.Windows.Forms;

    public partial class PiDebuggerInput
        : UserControlBase
    {
        public PiDebuggerInput()
        {
            InitializeComponent();
        }

        private void PiDebuggerInput_Load(object sender, EventArgs e)
        {

        }

        private void PiDebuggerInput_PreviewKeyDown(PreviewKeyDownEventArgs e)
        {
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

        public void Input(PreviewKeyDownEventArgs previewKeyDownEventArgs)
        {
            PiDebuggerInput_PreviewKeyDown(previewKeyDownEventArgs);
        }
    }
}

