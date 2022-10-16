﻿namespace Pyro.Impl
{
    using System;
    using System.Text;

    /// <inheritdoc cref="IClass{T}" />
    /// <inheritdoc cref="ClassBase" />
    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Class<T>
        : ClassBase
        , IClass<T>
    {
        private readonly Action<IRegistry, StringBuilder, T> _toText;

        internal Class(IRegistry reg)
            : base(reg, typeof(T))
        {
        }

        public Class(IRegistry reg, Action<IRegistry, StringBuilder, T> toText)
            : base(reg, typeof(T))
        {
            _toText = toText;
        }

        public override object NewInstance()
        {
            var instance = Activator.CreateInstance(Type);
            AddRefFields(instance);
            return instance;
        }

        public override void NewRef(Id id, out IRefBase refBase)
            => refBase = new Ref<T>(_registry, this, id);

        public IRef<T> NewRef(Id id, T value)
            => new Ref<T>(_registry, this, id, value);

        public IConstRef<T> CreateConst(Id id, T value)
            => new ConstRef<T>(_registry, this, id, value);

        public override void ToPiScript(StringBuilder str, object obj)
            => AppendText(str, _registry.Get<T>(obj));

        public void AppendText(StringBuilder str, T obj)
        {
            if (_toText != null)
                _toText(_registry, str, obj);
            else
                str.Append(obj.ToString());
        }
    }
}

