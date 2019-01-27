using System;
using System.Collections.Generic;
using System.Text;

namespace Pryo
{
    public interface IReflected
    {
        IRefBase SelfBase { get; set; }
    }

    public interface IReflected<T>
    {
        IRef<T> Self { get; }
    }

    public class ReflectedBase : IReflected
    {
        public IRefBase SelfBase { get; set; }
    }

    public class Reflected<T> : ReflectedBase, IReflected<T>
    {
        public IRef<T> Self
        {
            get => SelfBase as IRef<T>;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                SelfBase = Self = value;
            }
        }

        public IDictionary<string, object> Scope
        {
            get => _scope ?? (_scope = new Dictionary<string, object>());
            set => _scope = value;
        }

        public string ToText()
        {
            var str = new StringBuilder();
            Self.Class.AppendText(str, Self.Value);
            return str.ToString();
        }

        private IDictionary<string, object> _scope;
    }
}