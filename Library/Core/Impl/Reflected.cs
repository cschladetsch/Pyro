using System;
using System.Collections.Generic;
using System.Text;

namespace Pyro
{
    /// <inheritdoc cref="IReflected{T}" />
    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Reflected<T>
        : ReflectedBase
        , IReflected<T>
    {
        private IDictionary<string, object> _scope;

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
    }
}
