namespace Pyro.Exec {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Also known as a co-routine.
    /// Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation
    //        : IGenerator
    {
        /// <summary>
        /// The 'instruction pointer', or the thing to execute next in list of objects in code block.
        /// </summary>
        public int Ip { get; private set; }
        public IList<object> Code { get; set; }
        public IList<string> Args { get; private set; }
        private IDictionary<string, object> _scope => Scope;
        private IEnumerator _enumerator;

        public Continuation(IList<object> code) {
            Active = true;
            Running = true;
            Code = code;
        }

        private Continuation(IList<object> code, IList<string> args)
            : this(code) {
            Args = args;
        }

        public void Delay(int millis)
            => ResumeAfter(TimeSpan.FromMilliseconds(millis));

        //        public void Wait(ITransient other)
        //            => ResumeAfter(other);
        //
        /// <summary>
        /// Helper to make a new continuation, which also uses a referenced list for scope
        /// </summary>
        public static new Continuation New(IRegistry reg) {
            var code = reg.Add(new List<object>());
            return reg.Add(new Continuation(code.Value)).Value;
        }

        public static void ToText(IRegistry reg, StringBuilder str, Continuation cont) {
            str.Append('{');
            foreach (var elem in cont.Code) {
                switch (elem) {
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
        public override string ToString() {
            var str = new StringBuilder();
            str.Append('{');
            str.Append($"#{Ip}/{Code.Count} ");

            if (Args != null) {
                str.Append('(');
                var comma = "";
                foreach (var a in Args) {
                    str.Append($"{a}{comma}");
                    comma = ", ";
                }

                str.Append(") ");
            }

            foreach (var c in Code) {
                str.Append(c);
                str.Append(", ");
            }

            str.Append('}');

            return str.ToString();
        }

        public void AddArg(string ident) {
            if (Args == null)
                Args = new List<string>();

            Args.Add(ident);
        }

        public Continuation Start(Executor exec) {
            var cp = Self.Registry.Add(new Continuation(Code, Args)).Value;

            cp.Kernel = exec.Kernel;
            cp.Ip = Ip;
            cp.Scope = Scope;
            //cp.Kernel.Root.Add(cp);

            //            void End(ITransient tr)
            //            {
            //                exec.RemoveContinuation(this);
            //                cp.Completed -= End;
            //            }
            //
            //            cp.Completed += End;

            cp.Resumed += tr => {
                //exec.PushContext(cp);
                Info("Resumed coro");
            };

            cp.Suspended += tr => {
                Info("Suspended coro");
            };

            cp.Resume();

            if (Args != null) {
                if (exec.DataStack.Count < Args.Count)
                    throw new DataStackEmptyException($"Expected at least {Args.Count} objects on stack.");

                foreach (var arg in Args)
                    cp.Scope[arg] = exec.DataStack.Pop();
            }

            return cp;
        }

        public bool HasScopeObject(string label)
            => _scope.ContainsKey(label);

        public void SetScopeObject(string label, object val)
            => _scope[label] = val;

        public object FromScope(string label)
            => _scope.TryGetValue(label, out var value) ? value : null;

        public bool Next(out object next) {
            var has = Ip < Code.Count;
            next = has ? Code[Ip++] : null;
            if (has)
                return true;
            if (_enumerator == null)
                return false;
            if (!_enumerator.MoveNext())
                return false;

            next = _enumerator.Current;
            Ip = 0;
            return true;
        }

        //        IGenerator IGenerator.AddTo(IGroup @group)
        //        {
        //            throw new NotImplementedException();
        //        }
        //
        //        IGenerator IGenerator.Named(string name)
        //        {
        //            throw new NotImplementedException();
        //        }

        public void SetRange(IEnumerable range) {
            if (range == null) {
                _enumerator = null;
                return;
            }

            _enumerator = range.GetEnumerator();
        }

        public bool IsRunning()
            => Running && Active && (Ip < Code.Count || _enumerator != null);
    }
}

