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
            => Code = code;

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
        {
            reg.Register(new ClassBuilder<Continuation>(reg, ToText)
                .Class);
        }

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
        {
            return _scope.ContainsKey(label);
        }

        public void SetScopeObject(string label, object val)
        {
            _scope[label] = val;
        }

        public object FromScope(string label)
        {
            return _scope.TryGetValue(label, out var value) ? value : null;
        }

        public bool Next(out object next)
        {
            var has = Ip < Code.Count;
            next = has ? Code[Ip++] : null;
            if (!has)
                Reset();
            return has;
        }

        public void Reset()
        {
            // TODO: want to reset scope here, but also want to keep it to check results in unit-tests
            //_scope.Clear();
            Ip = 0;
        }

        public string Name { get; set; }
        public IKernel Kernel { get; set; }
        public event TransientHandler Completed;
        public bool Active { get; private set; }
        public void Complete()
        {
            if (!Active)
                return;
            Completed?.Invoke(this);
            Active = false;
            Running = false;
        }

        IGenerator IGenerator.AddTo(IGroup @group)
        {
            throw new NotImplementedException();
        }

        IGenerator IGenerator.Named(string name)
        {
            throw new NotImplementedException();
        }

        public IGenerator SuspendAfter(ITransient other)
        {
            throw new NotImplementedException();
        }

        public IGenerator SuspendAfter(TimeSpan span)
        {
            throw new NotImplementedException();
        }

        public IGenerator ResumeAfter(Func<bool> pred)
        {
            throw new NotImplementedException();
        }

        // TODO: this is duplicated code from Transient.ResumeAfter()
        public IGenerator ResumeAfter(ITransient other)
        {
            if (other == null || !other.Active)
            {
                Resume();
                return this;
            }

            Suspend();

            void DoResume(ITransient tr)
            {
                Resume();
                other.Completed -= DoResume;
            }

            other.Completed += DoResume;

            return this;
        }

        public IGenerator ResumeAfter(TimeSpan span)
        {
            throw new NotImplementedException();
        }

        public ITransient Named(string name)
        {
            Name = name;
            return this;
        }

        public event GeneratorHandler Resumed;
        public event GeneratorHandler Stepped;
        public event GeneratorHandler Suspended;
        public bool Running { get; protected set; }
        public int StepNumber { get; }
        public object Value { get; }
        public void Resume()
        {
            Active = true;
        }

        public void Pre()
        {
        }

        public void Post()
        {
        }

        public void Step()
        {
        }

        public void Suspend()
        {
            Active = false;
            Suspended?.Invoke(this);
        }

        public ITransient AddTo(IGroup @group)
        {
            @group.Add(this);
            return this;
        }

        public ITransient Then(IGenerator next)
        {
            throw new NotImplementedException();
        }

        public ITransient Then(Action action)
        {
            throw new NotImplementedException();
        }

        public ITransient Then(Action<ITransient> action)
        {
            throw new NotImplementedException();
        }
    }
}

