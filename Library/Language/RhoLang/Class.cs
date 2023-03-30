using System;
using Pyro.Exec;

namespace Pyro.RhoLang {
    /// <inheritdoc />
    /// <summary>
    ///     A Rho-space class type.
    ///     Such classes can be made with arguments.
    ///     They can also be created with arguments.
    ///     C# : <code>new Klass&lt;T&gt;(a0)</code> becomes
    ///     Rho: <code>Klass(T)(a0)</code>.
    ///     NOTE: not implemented.
    /// </summary>
    public class Class
        : Reflected<Class> {
        private readonly Executor _exec;

        private Class(Executor exec, Continuation def) {
            _exec = exec;

            // construct the meta-type by running the create continuation
            _exec.Continue(def);

            // now construct the actual type
            _exec.Continue(_exec.Pop<Continuation>());

            var klass = _exec.Pop<Continuation>();

            if (klass.Scope.TryGetValue("construct", out var ctor)) {
                CreateCont = ctor as Continuation;
            }

            if (klass.Scope.TryGetValue("destroy", out var dest)) {
                DestroyCont = dest as Continuation;
            }
        }

        /// <summary>
        ///     What to execute to create a new instance.
        /// </summary>
        private Continuation CreateCont { get; }

        /// <summary>
        ///     What to execute when an instance leaves scope.
        /// </summary>
        private Continuation DestroyCont { get; }

        /// <summary>
        ///     Create a new Rho-space runtime instance of this class.
        /// </summary>
        /// <returns></returns>
        public object Create() {
            _exec.Continue(CreateCont);
            if (!(_exec.Pop() is Continuation instance)) {
                throw new Exception($"Failed to make instance of Rho-Class {Name}");
            }

            instance.Scope["_class"] = this;
            return instance;
        }

        /// <summary>
        ///     Destroy an instance on the stack.
        /// </summary>
        public void Destroy() {
            _exec.Continue(DestroyCont);
        }
    }
}