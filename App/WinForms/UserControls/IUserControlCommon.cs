using Pyro.Exec;

namespace WinForms {
    public interface IUserControlCommon {
        Executor Executor { get; }

        IMainForm MainForm { get; set; }

        void Construct(IMainForm mainForm);
    }
}