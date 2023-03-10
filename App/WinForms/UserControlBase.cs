using Pyro;
using Pyro.Exec;
using System.Windows.Forms;

namespace WinForms {
    public class UserControlBase
        : UserControl
        , IUserControlCommon {
        public Executor Executor => MainForm.Executor;
        public IRegistry Registry => Executor.Self.Registry;

        public IMainForm MainForm { get; set; }

        Executor IUserControlCommon.Executor => throw new System.NotImplementedException();

        public virtual void Construct(IMainForm mainForm) {
            MainForm = mainForm;
        }

        protected void Perform(EOperation operation) {
            MainForm.Perform(operation);
        }
    }
}