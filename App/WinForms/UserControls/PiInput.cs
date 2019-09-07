using System.Diagnostics;
using Pyro.ExecutionContext;

namespace WinForms.UserControls
{
    using System.Windows.Forms;

    public partial class PiInput : UserControl
    {
        private MainForm _main;

        public PiInput()
        {
            InitializeComponent();
        }

        internal void SetMain(MainForm main)
        {
            _main = main;
        }

        private void PiInputKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                {
                    if (e.Control)
                    {
                        DebugPi();
                        e.Handled = true;
                    }

                    break;
                }
            }
        }

        private void DebugPi()
        {
            var text = richTextBox1;
            var pi = text.Lines[text.GetLineFromCharIndex(text.SelectionStart)];

            if (!_main.Context.Translate(pi, out var cont))
            {
                _main.Output.Text = _main.Context.Translator.Error + "\n\n" + _main.Output.Text;
                return;
            }

            _main.Context.Executor.PushContext(cont);
            _main.PiDebugger.Restart();

            //    Console
            //if (_local)
            //    Perform(() => _context.ExecPi(pi));
            //else
            //    Perform(() => _peer.Execute(pi));
        }
    }
}
