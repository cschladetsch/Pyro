using Pyro;
using Pyro.Exec;
using System.Windows.Forms;

namespace WinForms {
    public class UserControlBase
        : UserControl
        , IUserControlCommon {

        public Executor Executor { get { return MainForm.Executor; } }
        public IRegistry Registry => Executor.Self.Registry;

        public IMainForm MainForm { get { return _mainForm; } set { _mainForm = value; } }

        private IMainForm _mainForm;

        Executor IUserControlCommon.Executor => throw new System.NotImplementedException();

        public virtual void Construct(IMainForm mainForm) {
            MainForm = mainForm;
        }

        public virtual void Render() {
        }
    }
}