using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Diver.Exec
{
    public partial class Executor
    {
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
            _actions[EOperation.Not] = () => Push(!ResolvePop<bool>());
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

        private void LogicalOr()
        {
            var a = RPop();
            var b = RPop();
            var c = a || b;
            Push(c);
        }

        private void DebugPrintContextStack()
        {
            throw new NotImplementedException();
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
            {
                set.Add(RPop());
            }

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

        private T ConstRef<T>(object obj)
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
                {
                    Push(obj);
                }
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

        private void Divide()
        {
            var a = RPop();
            var b = RPop();
            Push(b / a);
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
                        throw new CannotEnumerateException(a, b);
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
