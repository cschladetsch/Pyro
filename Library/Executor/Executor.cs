using System;
using System.Collections.Generic;

namespace Diver.Exec
{
    public class Executor
    {
        public Stack<object> DataStack => _data;
        public Stack<IRef<Continuation>> ContextStack => _context;

        public Executor()
        {
            AddOperations();
        }

        private void AddOperations()
        {
            _actions[EOperation.Plus] = () => Push(Pop() + Pop());
            _actions[EOperation.Minus] = () => Push(Pop() - Pop());
            _actions[EOperation.Multiply] = () => Push(Pop() * Pop());
            _actions[EOperation.Divide] = () => Push(Pop() / Pop());

            _actions[EOperation.Suspend] = Suspend;
            _actions[EOperation.Resume] = Resume;
            _actions[EOperation.Replace] = Break;

            _actions[EOperation.Store] = () => Context().SetScopeObject(Pop<string>(), Pop());
            _actions[EOperation.Retrieve] = () => Push(Context().FromScope(Pop<string>()));
        }

        public void Continue(Continuation cont)
        {
            while (true)
            {
                while (cont.Next(out var next))
                {
                    if (next is IRef<EOperation> op)
                    {
                        if (_actions.TryGetValue(op.Value, out var action))
                        {
                            action();
                        }
                        else
                        {
                            throw new NotImplementedException($"Operation {op.Value}");
                        }
                    }
                    else
                    {
                        _data.Push(next);
                    }

                    if (_break)
                        break;
                }

                _break = false;

                if (_context.Count > 0)
                    Continue(_context.Pop());
                else
                    break;
            }
        }

        public void Continue(IRef<Continuation> cont)
        {
            if (cont == null)
                return;

            Continue(cont.Value);
        }

        private Continuation Context()
        {
            return _context.Peek().Value;
        }

        private void Suspend()
        {
            _context.Push(_context.Peek());
            Resume();
        }

        private void Resume()
        {
            _context.Push(Pop());
            Break();
        }

        private void Break()
        {
            _break = true;
        }

        private void Push(object obj)
        {
            _data.Push(obj);
        }

        private T Pop<T>()
        {
            var data = _data.Pop() as IRef<T>;
            return data.Value;
        }

        private dynamic Pop()
        {
            var pop = _data.Pop();
            return !(pop is IRefBase data) ? pop : data.BaseValue;
        }

        private bool _break;
        private Stack<object> _data = new Stack<object>();
        private Stack<IRef<Continuation>> _context = new Stack<IRef<Continuation>>();
        private Dictionary<EOperation, Action> _actions = new Dictionary<EOperation, Action>();
    }
}
