using System;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Diver.Exec
{
    public partial class Executor
    {
        public int FloatPrecision;

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
            _actions[EOperation.At] = At;
            _actions[EOperation.Has] = Has;
            _actions[EOperation.DebugPrintDataStack] = DebugPrintDataStack;
            _actions[EOperation.DebugPrintContinuation] = DebugPrintContinuation;
            _actions[EOperation.DebugPrintContextStack] = DebugPrintContextStack;
            _actions[EOperation.DebugPrint] = DebugTrace;
            _actions[EOperation.Depth] = () => Push(_data.Count);
            _actions[EOperation.SetFloatPrecision] = SetFloatPrecision;
            _actions[EOperation.Write] = () => Write(RPop());
            _actions[EOperation.WriteLine] = () => WriteLine(RPop());
            _actions[EOperation.If] = If;
            _actions[EOperation.IfElse] = IfElse;
            _actions[EOperation.Assign] = Assign;
            _actions[EOperation.GetMember] = GetMember;
            _actions[EOperation.ForEachIn] = ForEachIn;
            _actions[EOperation.ForLoop] = ForLoop;
        }

        private void ForLoop()
        {
            throw new NotImplementedException();
        }

        private void ForEachIn()
        {
            var block = Pop<Continuation>();
            var obj = RPop();
            var en = obj as IEnumerable;
            if (en == null)
                throw new CannotEnumerate(obj);
            var label = Pop<Label>();
            block.SetScopeObject(label.Text, null);
            foreach (var _ in ForEachInLoop(block, en, label.Text))
            {
                //Break();
                //_context.Pop();
            }
        }

        IEnumerable ForEachInLoop(Continuation block, IEnumerable obj, string label)
        {
            _context.Push(_current);
            foreach (var a in obj)
            {
                block.SetScopeObject(label, a);
                _current = block;
                _break = false;
                Execute(block);

                yield return 0;
            }

            _context.Pop();
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
            if (GetProperty(type, member, obj)) 
                return;
            if (GetMethod(type, member, obj, @class))
                return;
            GetCallable(@class, member, obj);
        }

        private bool GetProperty(Type type, string member, dynamic obj)
        {
            var pi = type.GetProperty(member);
            if (pi == null)
                return false;
            Push(pi.GetValue(obj));
            return true;
        }

        private bool GetMethod(Type type, string member, dynamic obj, IClassBase @class)
        {
            if (@class != null)
                return false;
            var mi = type.GetMethod(member);
            var numArgs = mi.GetParameters().Length;
            var args = DataStack.Take(numArgs).ToArray();
            Push(mi.Invoke(obj, args));
            return true;
        }

        private void GetCallable(IClassBase @class, string member, dynamic obj)
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
            // first, search context for an object with
            // matching name and use that
            foreach (var c in _context)
            {
                if (c.HasScopeObject(ident))
                {
                    _current.SetScopeObject(ident, val);
                    return;
                }
            }

            // if nothing found in context stack,
            // make new object in current scope
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
            var cont = RPop();
            var index = RPop();
            switch (cont)
            {
                case List<object> list:
                    Push(list[ConstRef<int>(index)]);
                    break;
                case Dictionary<object,object> dict:
                    Push(dict[RPop()]);
                    break;
                default:
                    throw new NotImplementedException($"Cannot use 'at' operation on a {cont.GetType().Name}");
            }
        }

        public static T ConstRef<T>(object obj)
        {
            if (obj is T result)
                return result;
            if (obj is IConstRef<T> cref)
                return cref.Value;
            throw new CannotConvertException(obj, typeof(T));
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

            // finally, push size of container
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
                case List<object> list:
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
                    var other = b as IEnumerable<object>;
                    if (other == null)
                        throw new CannotCompareEnumerationsException(a, b);
                    Push(list.SequenceEqual(other));
                    return;
                }
            }

            Push(a.Equals(b));
        }
    }

    public class CannotConvertException : Exception
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
