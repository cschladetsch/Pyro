namespace WinForms {
    using Pyro.Exec;

    public interface IUserControlCommon {
        Executor Executor { get; }
        IMainForm MainForm { get; set; }

        void Construct(IMainForm mainForm);
        void Render();
    }
}
