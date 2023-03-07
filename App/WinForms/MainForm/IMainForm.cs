namespace WinForms {
    using Pyro;
    using Pyro.Exec;

    public interface IMainForm {
        int ListenPort { get; }

        Executor Executor { get; }

        IRegistry Registry { get; }

        void Perform(EOperation op);
    }
}

