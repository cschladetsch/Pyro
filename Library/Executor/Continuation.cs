using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro.Exec
{
    /// <summary>
    /// Also known as a co-routine. Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation
        : Reflected<Continuation>
    {
        public IList<object> Code => _code;
        public IList<string> Args => _args;
        /// <summary>
        /// The 'instruction pointer', or the thing to execute next in list of objects in code block
        /// </summary>
        public int Ip => _next;

        public Continuation(IList<object> code)
        {
            _code = code;
        }

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
                        //str.Append('`');
                        //str.Append((int) op);
                        str.Append(OpToText(op));
                        break;
                    case bool val:
                        str.Append(val ? "true" : "false");
                        break;
                    default:
                        reg.AppendText(str, elem);
                        break;
                }

                str.Append(' ');
            }
            str.Append('}');
        }

        private static string OpToText(EOperation op)
        {
            switch (op)
            {
            case EOperation.Nop:
                return "nop";
            case EOperation.HasType:
                break;
            case EOperation.GarbageCollect:
                return "garbage_collect";
            case EOperation.Plus:
                return "+";
            case EOperation.Minus:
                return "-";
            case EOperation.Multiply:
                return "*";
            case EOperation.Divide:
                return "/";
            case EOperation.Modulo:
                return "%";
            case EOperation.Store:
                return "#";
            case EOperation.Has:
                return "has";
            case EOperation.Retrieve:
                return "@";
            case EOperation.Assign:
                break;
            case EOperation.GetPath:
                break;
            case EOperation.Suspend:
                return "&";
            case EOperation.Resume:
                return "...";
            case EOperation.Replace:
                return "!";
            case EOperation.Assert:
                return "assert";
            case EOperation.Write:
                return "write";
            case EOperation.WriteLine:
                return "writeln";
            case EOperation.If:
                return "?";
            case EOperation.IfElse:
                break;
            case EOperation.StackToList:
                break;
            case EOperation.ListToStack:
                break;
            case EOperation.Depth:
                break;
            case EOperation.Dup:
                return "dup";
            case EOperation.Clear:
                return "clear";
            case EOperation.Swap:
                return "swap";
            case EOperation.Break:
                return "break";
            case EOperation.Rot:
                break;
            case EOperation.Roll:
                break;
            case EOperation.RotN:
                break;
            case EOperation.RollN:
                break;
            case EOperation.Pick:
                break;
            case EOperation.Over:
                break;
            case EOperation.Freeze:
                break;
            case EOperation.Thaw:
                break;
            case EOperation.FreezeText:
                break;
            case EOperation.ThawText:
                break;
            case EOperation.FreezeYaml:
                break;
            case EOperation.ThawYaml:
                break;
            case EOperation.Not:
                break;
            case EOperation.Equiv:
                break;
            case EOperation.LogicalAnd:
                break;
            case EOperation.LogicalOr:
                break;
            case EOperation.LogicalXor:
                break;
            case EOperation.Less:
                break;
            case EOperation.Greater:
                break;
            case EOperation.GreaterOrEquiv:
                break;
            case EOperation.LessOrEquiv:
                break;
            case EOperation.NotEquiv:
                break;
            case EOperation.Expand:
                break;
            case EOperation.ToArray:
                break;
            case EOperation.ToMap:
                break;
            case EOperation.ToSet:
                break;
            case EOperation.ToPair:
                break;
            case EOperation.Size:
                break;
            case EOperation.GetBack:
                break;
            case EOperation.PushBack:
                break;
            case EOperation.PushFront:
                break;
            case EOperation.ToList:
                break;
            case EOperation.Remove:
                break;
            case EOperation.Insert:
                break;
            case EOperation.At:
                break;
            case EOperation.DebugPrintDataStack:
                break;
            case EOperation.DebugPrintContextStack:
                break;
            case EOperation.DebugPrint:
                break;
            case EOperation.DebugPrintContinuation:
                break;
            case EOperation.DebugSetLevel:
                break;
            case EOperation.SetFloatPrecision:
                break;
            case EOperation.Self:
                break;
            case EOperation.GetMember:
                break;
            case EOperation.SetMember:
                break;
            case EOperation.SetMemberValue:
                break;
            case EOperation.ForEachIn:
                break;
            case EOperation.ForLoop:
                break;
            case EOperation.Drop:
                break;
            case EOperation.DropN:
                break;
            }

            return $"`{(int) op}";
        }

        public static void Register(IRegistry reg)
        {
            reg.Register(new ClassBuilder<Continuation>(reg, ToText)
                .Class);
        }

        // this is human-readable version. for transmission/persistence, use ToText()
        public override string ToString()
        {
            var str = new StringBuilder();
            //str.Append($"Continuation: {_scope.Count} args, {_code.Count} instructions:\n\t");
            str.Append('{');
            str.Append($"#{_next}/{_code.Count} ");
            if (_args != null)
            {
                str.Append('(');
                var comma = "";
                foreach (var a in _args)
                {
                    str.Append($"{a}{comma}");
                    comma = ", ";
                }
                str.Append(") ");
            }

            foreach (var c in _code)
            {
                str.Append(c);
                str.Append(", ");
            }
            str.Append('}');

            return str.ToString();
            //return $"#{_next}/{_code.Count}:{ToText()}";
        }

        public void AddArg(string ident)
        {
            if (_args == null)
                _args = new List<string>();
            _args.Add(ident);
        }

        public void Enter(Executor exec)
        {
            if (_args == null)
                return;

            // already entered
            if (_next != 0)
                return;

            if (exec.DataStack.Count < _args.Count)
                throw new DataStackEmptyException();
            
            foreach (var arg in _args)
                _scope[arg] = exec.DataStack.Pop();

            _next = 0;
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
            var has = _next < _code.Count;
            next = has ? _code[_next++] : null;
            if (!has)
                Reset();
            return has;
        }

        public void Reset()
        {
            // TODO: want to reset scope here, but also want to keep it to check results in unit-tests
            //_scope.Clear();
            _next = 0;
        }

        private int _next;
        private IList<string> _args;
        private readonly IList<object> _code;
        private IDictionary<string, object> _scope => Scope;
    }
}
