using Pyro.Language;

namespace WinForms {
    using Pyro;
    using Pyro.Exec;
    using WinForms.UserControls;

    public interface IMainForm {
        int ListenPort { get; }

        Executor Executor { get; }

        IRegistry Registry { get; }

        ContextStackView ContextView { get; }

        void Perform(EOperation op);
        void ConvertToPi();
        void Decompile();
        void RunCurrent();
        void Run(string text, ELanguage language);
    }
}

