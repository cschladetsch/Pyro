using System.Text;
using System.Collections.Generic;

namespace Pyro.Exec
{
    /// <summary>
    /// Also known as a co-routine.
    ///
    /// Can be interrupted mid-execution and later resumed.
    /// </summary>
    public partial class Continuation
    {
        /// <summary>
        /// The 'instruction pointer', or the thing to execute next in list of objects in code block
        /// </summary>
        public int Ip { get; private set; }
        public IList<object> Code { get; }
        public IList<string> Args { get; private set; }

        private IDictionary<string, object> _scope => Scope;

        public Continuation(IList<object> code)
        {
            Code = code;
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
                return "if";
            case EOperation.IfElse:
                return "ife";
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
                return "roll";
            case EOperation.RotN:
                return "rotn";
            case EOperation.RollN:
                return "rolln";
            case EOperation.Pick:
                return "pick";
            case EOperation.Over:
                return "over";
            case EOperation.Freeze:
                return "freeze";
            case EOperation.Thaw:
                return "thaw";
            case EOperation.FreezeText:
                return "freezet";
            case EOperation.ThawText:
                return "thawt";
            case EOperation.FreezeYaml:
                return "freezey";
            case EOperation.ThawYaml:
                return "thawy";
            case EOperation.Not:
                return "!";
            case EOperation.Equiv:
                return "==";
            case EOperation.LogicalAnd:
                return "&&";
            case EOperation.LogicalOr:
                return "||";
            case EOperation.LogicalXor:
                return "^";
            case EOperation.Less:
                return "<";
            case EOperation.Greater:
                return ">";
            case EOperation.GreaterOrEquiv:
                return ">=";
            case EOperation.LessOrEquiv:
                return "<=";
            case EOperation.NotEquiv:
                return "!=";
            case EOperation.Expand:
                return "expand";
            case EOperation.ToArray:
                return "to_array";
            case EOperation.ToMap:
                return "to_map";
            case EOperation.ToSet:
                return "to_set";
            case EOperation.ToPair:
                return "to_pair";
            case EOperation.Size:
                return "size";
            case EOperation.GetBack:
                return "back";
            case EOperation.PushBack:
                return "push_back";
            case EOperation.PushFront:
                return "push_front";
            case EOperation.ToList:
                return "tolist";
            case EOperation.Remove:
                return "remove";
            case EOperation.Insert:
                return "insert";
            case EOperation.New:
                return "new";
            case EOperation.At:
                return "@";
            case EOperation.DebugPrintDataStack:
                return "debug_print_data";
            case EOperation.DebugPrintContextStack:
                return "debug_print_context";
            case EOperation.DebugPrint:
                return "debug_print";
            case EOperation.DebugPrintContinuation:
                return "debug_print_cont";
            case EOperation.DebugSetLevel:
                return "set_debug_level";
            case EOperation.SetFloatPrecision:
                return "set_float_precision";
            case EOperation.Self:
                return "self";
            case EOperation.GetMember:
                return "get_member";
            case EOperation.SetMember:
                return "set_member";
            case EOperation.SetMemberValue:
                return "set_member_value";
            case EOperation.ForEachIn:
                return "for_each";
            case EOperation.ForLoop:
                return "for";
            case EOperation.Drop:
                return "drop";
            case EOperation.DropN:
                return "dropn";
            }

            return $"`{(int) op}";
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
    }
}

