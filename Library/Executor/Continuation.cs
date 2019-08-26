namespace Pyro.Exec
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using Flow;

    /// <summary>
    /// Also known as a co-routine.
    /// Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation
        : IGenerator
    {
        /// <summary>
        /// The 'instruction pointer', or the thing to execute next in list of objects in code block.
        /// </summary>
        public int Ip { get; private set; }
        public IList<object> Code { get; }
        public IList<string> Args { get; private set; }
        private IDictionary<string, object> _scope => Scope;

        public Continuation(IList<object> code)
        {
            Active = true;
            Running = true;
            Code = code;
        }

        public void Delay(int millis)
            => ResumeAfter(TimeSpan.FromMilliseconds(millis));

        public void Wait(ITransient other)
            => ResumeAfter(other);

        /// <summary>
        /// Helper to make a new continuation, which also uses a referenced list for scope
        /// </summary>
        public static Continuation New(IRegistry reg)
        {
            var code = reg.Add(new List<object>());
            return reg.Add(new Continuation(code.Value)).Value;
        }

        public static void ToText(IRegistry reg, StringBuilder str, Continuation cont)
        {
            str.Append('{');
            foreach (var elem in cont.Code)
            {
                switch (elem)
                {
                    case EOperation op:
                        str.Append(OpToText(op));
                        break;

                    case bool val:
                        str.Append(val ? "true" : "false");
                        break;

                    default:
                        reg.ToPiScript(str, elem);
                        break;
                }

                str.Append(' ');
            }

            str.Append('}');
        }

        public static void Register(IRegistry reg)
            => reg.Register(new ClassBuilder<Continuation>(reg, ToText)
            .Class);

        // this is human-readable version. for transmission/persistence, use ToPiScript()
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append('{');
            str.Append($"#{Ip}/{Code.Count} ");

            if (Args != null)
            {
                str.Append('(');
                var comma = "";
                foreach (var a in Args)
                {
                    str.Append($"{a}{comma}");
                    comma = ", ";
                }

                str.Append(") ");
            }

            foreach (var c in Code)
            {
                str.Append(c);
                str.Append(", ");
            }

            str.Append('}');

            return str.ToString();
        }

        public void AddArg(string ident)
        {
            if (Args == null)
                Args = new List<string>();

            Args.Add(ident);
        }

        public void Enter(Executor exec)
        {
            if (Kernel == null)
            {
                Kernel = exec.Kernel;
                Kernel.Root.Add(this);

                void End(ITransient tr)
                {
                    exec.RemoveContinuation(this);
                    Completed -= End;
                }
                Completed += End;

                Resumed += tr =>
                {
                    Info("Resumed coro");
                };
                Suspended += tr =>
                {
                    exec.PushContext(this);
                    Info("Suspended coro");
                };

                Running = true;
            }

            // Nothing to do if no args to pull.
            if (Args == null)
                return;

            // Already entered; we may be re-entering, which is fine.
            if (Ip != 0)
                return;

            if (exec.DataStack.Count < Args.Count)
                throw new DataStackEmptyException();

            foreach (var arg in Args)
                _scope[arg] = exec.DataStack.Pop();

            Ip = 0;
        }

        public bool HasScopeObject(string label)
            => _scope.ContainsKey(label);

        public void SetScopeObject(string label, object val)
            => _scope[label] = val;

        public object FromScope(string label)
            => _scope.TryGetValue(label, out var value) ? value : null;

        public bool Next(out object next)
        {
            var has = Ip < Code.Count;
            next = has ? Code[Ip++] : null;
            if (Ip == Code.Count)
                Reset();
            return has;
        }

        public void Reset()
        {
            //Complete();
            Running = false;

            // TODO: want to reset scope here, but also want to keep it to check results in unit-tests
            //_scope.Clear();
            Ip = 0;
        }

        IGenerator IGenerator.AddTo(IGroup @group)
        {
            throw new NotImplementedException();
        }

        IGenerator IGenerator.Named(string name)
        {
            throw new NotImplementedException();
        }
    }
}

