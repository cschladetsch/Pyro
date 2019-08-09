namespace Pyro.Exec
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// Implementation of the various operations an Executor can perform.
    /// </summary>
    public partial class Executor
    {
        /// <summary>
        /// The number of significant digits for float comparisons.
        /// </summary>
        public int FloatPrecision;

        /// <summary>
        /// A hack that is broken anyway.
        /// </summary>
        private bool _leaveForEach;

        /// <summary>
        /// Add options to the internal mapping of EOperation enum to
        /// functor that does the work for that operation.
        /// </summary>
        private void AddOperations()
        {
            _actions[EOperation.Plus] = () =>
            {
                var b = RPop();
                var a = RPop();
                Push(a + b);
            };

            _actions[EOperation.Minus] = () =>
            {
                var a = RPop();
                var b = RPop();
                Push(b - a);
            };

            _actions[EOperation.Multiply] = () => Push(RPop() * RPop());
            _actions[EOperation.Divide] = Divide;
            _actions[EOperation.Suspend] = Suspend;
            _actions[EOperation.Resume] = Resume;
            _actions[EOperation.Replace] = Break;
            _actions[EOperation.Break] = DebugBreak;
            _actions[EOperation.Store] = StoreValue;
            _actions[EOperation.Retrieve] = GetValue;
            _actions[EOperation.Assert] = Assert;
            _actions[EOperation.Equiv] = Equiv;
            _actions[EOperation.NotEquiv] = NotEquiv;
            _actions[EOperation.Not] = () => Push(!RPop<bool>());
            _actions[EOperation.LogicalAnd] = LogicalAnd;
            _actions[EOperation.LogicalOr] = LogicalOr;
            _actions[EOperation.LogicalXor] = LogicalXor;
            _actions[EOperation.ToArray] = ToArray;
            _actions[EOperation.ToList] = ToArray;
            _actions[EOperation.ToMap] = ToMap;
            _actions[EOperation.ToSet] = ToSet;
            _actions[EOperation.Size] = GetSize;
            _actions[EOperation.PushFront] = PushFront;
            _actions[EOperation.PushBack] = PushBack;
            _actions[EOperation.Remove] = Remove;
            _actions[EOperation.Expand] = Expand;
            _actions[EOperation.Insert] = Insert;
            _actions[EOperation.New] = New;
            _actions[EOperation.GetType] = GetTypeOf;
            _actions[EOperation.At] = At;
            _actions[EOperation.Has] = Has;
            _actions[EOperation.DebugPrintDataStack] = DebugPrintDataStack;
            _actions[EOperation.DebugPrintContinuation] = DebugPrintContinuation;
            _actions[EOperation.DebugPrintContextStack] = DebugPrintContextStack;
            _actions[EOperation.DebugPrint] = DebugTrace;
            _actions[EOperation.Depth] = () => Push(DataStack.Count);
            _actions[EOperation.SetFloatPrecision] = SetFloatPrecision;
            _actions[EOperation.Write] = () => Write(RPop());
            _actions[EOperation.WriteLine] = () => WriteLine(RPop());
            _actions[EOperation.If] = If;
            _actions[EOperation.IfElse] = IfElse;
            _actions[EOperation.Assign] = Assign;
            _actions[EOperation.GetMember] = GetMember;
            _actions[EOperation.ForEachIn] = ForEachIn;
            _actions[EOperation.ForLoop] = ForLoop;
            _actions[EOperation.Freeze] = Freeze;
            _actions[EOperation.Thaw] = Thaw;
            _actions[EOperation.Drop] = () => Pop();
            _actions[EOperation.DropN] = DropN;
            _actions[EOperation.Swap] = Swap;
            _actions[EOperation.Pick] = Pick;
            _actions[EOperation.Rot] = Rot;
            _actions[EOperation.Over] = Over;
            _actions[EOperation.Dup] = Dup;
            _actions[EOperation.Clear] = () => DataStack.Clear();
            _actions[EOperation.Less] = Less;
            _actions[EOperation.LessOrEquiv] = LessEquiv;
            _actions[EOperation.Greater] = Greater;
            _actions[EOperation.GreaterOrEquiv] = GreaterEquiv;
        }

        public void Clear()
        {
            DataStack = new Stack<object>();
            ContextStack = new Stack<Continuation>();
            NumOps = 0;

            _break = false;
            _current = null;
        }

        private void Assert()
        {
            if (!Pop<bool>())
                throw new AssertionFailedException();
        }

        private void StoreValue()
        {
            var name = Pop<IdentBase>();
            var val = Pop();
            if (name is Label label)
                Context().SetScopeObject(label.Text, val);
            else
                throw new Exception($"Can't store to {name}");
        }

        private void GetValue()
        {
            var label = Pop<string>();
            var fromScope = Context().FromScope(label);
            Push(fromScope);
        }

        private void GetTypeOf()
        {
            var obj = Pop();
            if (obj is IConstRefBase cref)
                Push(cref.Class.TypeName);
            Push(obj.GetType());
        }

        private void GreaterEquiv()
        {
            var b = Pop();
            var a = Pop();
            Push(a >= b);
        }

        private void Greater()
        {
            var b = Pop();
            var a = Pop();
            Push(a > b);
        }

        private void LessEquiv()
        {
            var b = Pop();
            var a = Pop();
            Push(a <= b);
        }

        private void Less()
        {
            var b = Pop();
            var a = Pop();
            Push(a < b);
        }

        private void Dup()
        {
            var top = Pop();
            var dup = top;//Duplicate(top); // TODO: copy-on-write duplication
            Push(top);
            Push(dup);
        }

        private object Duplicate(object obj)
        {
            return _registry.Duplicate(obj);
        }

        private void Over()
        {
            var a = Pop();
            var b = Pop();
            Push(b);
            Push(a);
            Push(b);
        }

        private void Rot()
        {
            // 1 2 3 rot => 3 1 2
            var a = Pop();
            var b = Pop();
            var c = Pop();
            Push(a);
            Push(c);
            Push(b);
        }

        private void DropN()
        {
            var n = Pop<int>();
            while (n-- > 0)
                Pop();
        }

        private void Pick()
        {
            var n = Pop<int>();
            Push(DataStack.ToArray()[n]);
        }

        private void Swap()
        {
            var a = Pop();
            var b = Pop();
            Push(a);
            Push(b);
        }

        private static void Thaw()
        {
            throw new NotImplementedException();
        }

        private void Freeze()
        {
            Push(_current.ToText());
        }

        private static void ForLoop()
        {
            throw new NotImplementedException();
        }

        private void ForEachIn()
        {
            var block = Pop<Continuation>();
            var obj = RPop();
            if (!(obj is IEnumerable en))
                throw new CannotEnumerate(obj);

            var label = Pop<Label>().Text;
            block.SetScopeObject(label, null);
            foreach (var _ in ForEachInLoop(block, en, label))
            {
            }
        }

        private IEnumerable ForEachInLoop(Continuation block, IEnumerable obj, string label)
        {
            var next = obj.GetEnumerator();
            if (!next.MoveNext())
                yield break;

            ContextStack.Push(_current);

            // We need to ensure that when an inner loop ends,
            // the outer loop doesn't move to next value in the
            // enumeration.
            //
            // This is what _leaveForEach is used for. It's
            // a bit complicated, sorry.
            while (true)
            {
                var val = next.Current;
                block.SetScopeObject(label, val);
                _current = block;
                _break = false;
                Execute(block);

                if (!_leaveForEach && !next.MoveNext())
                {
                    _leaveForEach = true;
                    break;
                }

                _leaveForEach = false;
                yield return val;
            }

            // TODO: pop or not?!?
            ContextStack.Pop();
            Break();
        }

        private void OpNew()
        {
            var obj = RPop();
            var @class = ConstRef<IClassBase>(RPop());
            if (@class == null)
                throw new Exception($"Couldn't get class from {obj}");

            Push(_registry.New(@class, DataStack));
        }

        private void GetMember()
        {
            var obj = Pop();
            var member = Pop<Label>().Text;
            var type = (Type)obj.GetType();
            var @class = _registry.GetClass(type);

            if (GetField(type, member, obj))
                return;

            if (GetProperty(type, member, obj))
                return;

            if (GetMethod(type, member, obj, @class))
                return;

            GetCallable(@class, member, obj);
        }

        private bool GetField(Type type, string member, object obj)
        {
            var field = type.GetField(member);
            if (field == null)
                return false;

            Push(field.GetValue(obj));
            return true;
        }

        private bool GetProperty(Type type, string member, object obj)
        {
            var pi = type.GetProperty(member);
            if (pi == null)
                return false;

            Push(pi.GetValue(obj));
            return true;
        }

        private bool GetMethod(Type type, string member, object obj, IClassBase @class)
        {
            if (@class == null)
                return false;
            if (type == null)
                return false;

            var mi = type.GetMethod(member);
            if (mi == null)
                return false;

            Push(obj);
            Push(mi);

            return true;
        }

        private void GetCallable(IClassBase @class, string member, object obj)
        {
            var callable = @class.GetCallable(member);
            if (callable == null)
                throw new MemberNotFoundException(obj.GetType(), member);

            Push(obj);
            Push(callable);
        }

        private void NotEquiv()
        {
            Equiv();
            Push(!Pop());
        }

        private void Assign()
        {
            var ident = Pop<Label>().Text;
            var val = RPop();
            // First, search context for an object with
            // matching name and use that.
            foreach (var c in ContextStack)
            {
                if (!c.HasScopeObject(ident))
                    continue;

                c.SetScopeObject(ident, val);
                return;
            }

            // If nothing found in context stack,
            // make new object in current scope.
            _current.SetScopeObject(ident, val);
        }

        private void IfElse()
        {
            var test = RPop<bool>();
            var thenBody = RPop();
            var ifBody = RPop();

            Push(test ? ifBody : thenBody);
        }

        private void If()
        {
            var test = RPop<bool>();
            var ifBody = RPop();
            if (test)
                Push(ifBody);
        }

        private void SetFloatPrecision()
        {
            FloatPrecision = RPop<int>();
        }

        private void LogicalXor()
        {
            var a = RPop();
            var b = RPop();
            Push(a ^ b);
        }

        private void LogicalAnd()
        {
            var a = RPop();
            var b = RPop();
            Push(a && b);
        }

        private void Divide()
        {
            var a = RPop();
            var b = RPop();
            Push(b / a);
        }

        private void LogicalOr()
        {
            var a = RPop();
            var b = RPop();
            Push(a || b);
        }

        private void DebugPrintContextStack()
        {
            //WriteContextStack();
            throw new NotImplementedException("DebugPrintContextStack");
        }

        private void DebugPrintContinuation()
        {
            WriteContinuation();
        }

        private void DebugPrintDataStack()
        {
            WriteDataStack(100);
        }

        private void ToSet()
        {
            var count = RPop<int>();
            var set = new HashSet<object>();
            while (count-- > 0)
                set.Add(RPop());

            Push(set);
        }

        private void ToMap()
        {
            var count = RPop<int>();
            var dict = new Dictionary<object, object>();
            while (count-- > 0)
            {
                var value = RPop();
                var key = RPop();

                dict.Add(key, value);
            }

            Push(dict);
        }

        private void Has()
        {
            var cont = RPop();
            switch (cont)
            {
                case List<object> list:
                    Push(list.Contains(RPop()));
                    break;

                case Dictionary<object, object> dict:
                    Push(dict.ContainsKey(RPop()));
                    break;

                case HashSet<object> set:
                    Push(set.Contains(RPop()));
                    break;

                 default:
                    throw new NotImplementedException($"Cannot use 'has' on type {cont.GetType().Name}");
            }
        }

        private void At()
        {
            var index = RPop();
            var cont = RPop();
            switch (cont)
            {
                case IList list:
                    Push(list[ConstRef<int>(index)]);
                    break;

                case IDictionary dict:
                    Push(dict[RPop()]);
                    break;

                default:
                    throw new NotImplementedException($"Cannot use 'at' operation on a {cont.GetType().Name}");
            }
        }

        public static T ConstRef<T>(object obj)
        {
            switch (obj)
            {
            case T result:
                return result;
            case IConstRef<T> cref:
                return cref.Value;
            }

            throw new CannotConvertException(obj, typeof(T));
        }

        private void New()
        {
            var typeName = Pop<Label>();
            var klass = _registry.GetClass(typeName.Text);
            if (klass == null)
                throw new UnknownIdentifierException(typeName);

            Push(_registry.New(klass, DataStack));
        }

        private void Insert()
        {
            var cont = RPop();
            switch (cont)
            {
                case List<object> list:
                    var index = RPop();
                    list.Insert(index, RPop());
                    break;

                case Dictionary<object, object> map:
                    var kv = RPop();
                    map.Add(kv.Key, kv.Value);
                    break;

                case HashSet<object> set:
                    set.Add(RPop());
                    break;

                default:
                    throw new NotImplementedException($"Cannot insert into {cont.GetType().Name}");
            }
        }

        private void Expand()
        {
            var cont = RPop();
            if (cont is Dictionary<object, object> dict)
            {
                foreach (var kv in dict)
                {
                    Push(kv.Key);
                    Push(kv.Value);
                }
            }
            else
            {
                foreach (var obj in cont)
                    Push(obj);
            }

            // Push size of container.
            Push(cont);
            GetSize();
        }

        private void Remove()
        {
            var cont = RPop();
            var index = RPop();
            switch (cont)
            {
                case List<object> list:
                    list.RemoveAt(index);
                    break;

                case Dictionary<object, object> dict:
                    dict.Remove(index);
                    break;

                case HashSet<object> set:
                    set.Remove(index);
                    break;

                default:
                    throw new NotImplementedException($"Cannot remove {index} from type {cont.GetType().Name}");
            }

            Push(cont);
        }

        private void GetSize()
        {
            var cont = RPop();
            switch (cont)
            {
                case IList list:
                    Push(list.Count);
                    return;

                case Dictionary<object, object> dict:
                    Push(dict.Count);
                    return;

                case HashSet<object> set:
                    Push(set.Count);
                    return;

               default:
                    throw new NotImplementedException($"Cannot get size of a {cont.GetType().Name}");
            }
        }

        private void PushBack()
        {
            var cont = RPop<List<object>>() as List<object>;
            var obj = RPop();
            cont.Add(obj);
            Push(cont);
        }

        private void PushFront()
        {
            var cont = RPop<List<object>>() as List<object>;
            var obj = RPop();
            cont.Insert(0, obj);
            Push(cont);
        }

        private void ToArray()
        {
            var count = RPop<int>();
            var list = new List<object>();
            while (count-- > 0)
                list.Add(RPop());

            list.Reverse();
            Push(list);
        }

        private void Equiv()
        {
            var a = RPop();
            var b = RPop();
            switch (a)
            {
                case IEnumerable<object> list:
                {
                    if (!(b is IEnumerable<object> other))
                        throw new CannotCompareEnumerationsException(a, b);

                    Push(list.SequenceEqual(other));
                    return;
                }
            }

            Push(a.Equals(b));
        }
    }

    /// <summary>
    /// DOC
    /// </summary>
    public class CannotConvertException
        : Exception
    {
        public object Object;
        public Type TargetType;

        public CannotConvertException(object obj, Type type)
        {
            Object = obj;
            TargetType = type;
        }

        public override string ToString()
        {
            return $"Couldn't convert {Object} to type {TargetType.Name}";
        }
    }
}

