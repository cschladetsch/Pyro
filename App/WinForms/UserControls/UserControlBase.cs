namespace WinForms.UserControls
{
    using System.Windows.Forms;
    using System.Collections.Generic;
    using Pyro.Exec;

    /// <inheritdoc />
    /// <summary>
    /// Common to all user-controls.
    /// </summary>
    public class UserControlBase
        : UserControl
    {
        protected MainForm _Main;
        protected Executor _Exec => _Main.Context.Executor;
        protected Stack<object> _Data => _Exec.DataStack;
        protected List<Continuation> _Context => _Exec.ContextStack;

        protected PiDebugger _piDebugger => _Main.PiDebugger;
        protected ContextStackView _contextStackView => _Main.ContextStackView;

        internal virtual void Construct(MainForm main)
        {
            _Main = main;
        }

        public virtual void Clear()
        {
        }
    }
}

