namespace WinForms.UserControls {
    using System;

    namespace WinForms {

        public partial class PiDebugger
            : UserControlBase
            , IUserControlCommon {
            //private MainForm _main;

            private void PiDebugger_Load(object sender, EventArgs e) {
            }

            public override void Render() {
            }

            public void Input(string pi) {
                Console.WriteLine($"Debugging {pi}");
            }

            public override void Construct(IMainForm mainForm) {
                throw new NotImplementedException();
            }
        }
    }
}
