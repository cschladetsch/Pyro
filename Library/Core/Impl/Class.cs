using System;
using System.Collections.Generic;
using System.Text;

namespace Pryo.Impl
{
    public class Class<T>
        : ClassBase, IClass<T> 
    {
        internal Class(IRegistry reg)
            : base(reg, typeof(T))
        {
        }

        public Class(IRegistry reg, Action<IRegistry, StringBuilder, T> toText)
            : base(reg, typeof(T))
        {
            this._toText = toText;
        }

        public override object NewInstance(Stack<object> stack)
        {
            return Activator.CreateInstance(Type);
        }

        public override void NewRef(Id id, out IRefBase refBase)
        {
            refBase = new Ref<T>(_registry, this, id);
        }

        public IRef<T> NewRef(Id id, T value)
        {
            return new Ref<T>(_registry, this, id, value);
        }

        public IConstRef<T> CreateConst(Id id, T value)
        {
            return new ConstRef<T>(_registry, this, id, value);
        }

        public override void AppendText(StringBuilder str, object obj)
        {
            AppendText(str, _registry.Get<T>(obj));
        }

        public void AppendText(StringBuilder str, T obj)
        {
            if (_toText != null)
                _toText(_registry, str, obj);
            else
                str.Append(obj.ToString());
        }

        private readonly Action<IRegistry, StringBuilder, T> _toText;
    }
}
