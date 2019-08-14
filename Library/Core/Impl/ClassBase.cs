using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework.Internal;

namespace Pyro.Impl
{
    /// <inheritdoc cref="IClassBase" />
    /// <inheritdoc cref="StructBase" />
    /// <summary>
    /// Common to all Class types used by a Registry.
    /// </summary>
    public class ClassBase
        : StructBase
        , IClassBase
    {
        public int TypeNumber { get; set; }

        private readonly Dictionary<string, ICallable> _callables = new Dictionary<string, ICallable>();

        internal ClassBase(IRegistry reg, Type type)
            : base(reg, type)
        {
            //foreach (var method in type.GetMethods())
            //{
            //    var call = MakeCallable(method);
            //    if (call == null)
            //        continue;

            //    _callables[method.Name] = call;
            //}
        }

        private ICallable MakeCallable(MethodInfo mi)
        {
            Type gen;
            var parameters = mi.GetParameters();
            var pars = parameters.Select(p => p.ParameterType).ToArray();
            var returnType = mi.ReturnType;
            if (returnType == typeof(void))
            {
                switch (parameters.Length)
                {
                case 0:
                    gen = typeof(VoidMethod<>).MakeGenericType(Type);
                    break;
                case 1:
                    gen = typeof(VoidMethod<,>).MakeGenericType(Type, pars[0]);
                    break;
                case 2:
                    gen = typeof(VoidMethod<,,>).MakeGenericType(Type, pars[0], pars[1]);
                    break;
                default:
                    return null;
                }
            }
            else
            {
                switch (parameters.Length)
                {
                case 0:
                    gen = typeof(Method<,>).MakeGenericType(Type, returnType);
                    break;
                case 1:
                    gen = typeof(Method<,,>).MakeGenericType(Type, pars[0], returnType);
                    var d = typeof(Func<,,>).MakeGenericType(Type, pars[0], returnType);
                    Action<object, object> a = (q, a0) => mi.Invoke(q, new[] {a0});

                    break;
                case 2:
                    gen = typeof(Method<,,,>).MakeGenericType(Type, pars[0], pars[1], returnType);
                    break;
                default:
                    return null;
                }
            }

            var p2 = mi.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();
            var call = Expression.Call(null, mi, p2);
            var del = Expression.Lambda(call, p2).Compile();

            return Activator.CreateInstance(gen, del) as ICallable;
        }

        public object Duplicate(object obj)
        {
            return null;
        }

        public ICallable GetCallable(string name)
        {
            return _callables.TryGetValue(name, out var call) ? call : null;
        }

        public void AddCallable(string name, ICallable callable)
        {
            if (_callables.ContainsKey(name))
                throw new Exception("Duplicate callable added to class");

            _callables[name] = callable;
        }

        public virtual void NewRef(Id id, out IRefBase refBase)
        {
            refBase = new RefBase(_registry, this, id);
        }

        protected void AddRefFields(object instance)
        {
            foreach (var field in _fields)
            {

            }
        }

        public IRefBase Create(Id id, object value)
        {
            return new RefBase(_registry, this, id, value);
        }

        public IConstRefBase CreateConst(Id id)
        {
            return new ConstRefBase(_registry, this, id);
        }

        public IConstRefBase CreateConst(Id id, object value)
        {
            return new ConstRefBase(_registry, this, id, value);
        }

        public virtual object NewInstance()
        {
            throw new NotImplementedException();
        }

        public virtual object NewInstance(Stack<object> dataStack)
        {
            throw new NotImplementedException();
        }

        public virtual void ToPiScript(StringBuilder str, object value)
        {
            str.Append(value);
        }
    }
}

