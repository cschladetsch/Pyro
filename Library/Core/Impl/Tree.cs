using System;
using System.Collections.Generic;

namespace Pyro.Impl
{
    internal class Tree
        : ITree
    {
        public IConstRefBase Root { get; set; }
        public IConstRefBase Scope { get; set; }
        public IConstRefBase Parent { get; }
        public IList<IConstRefBase> SearchPath { get; set; }

        private IConstRefBase _scope;

        public object Resolve(IIdentifer ident)
        {
            if (!(Scope is IConstRefBase cr))
                return GetNative(ident.ToText());
            if (cr.Scope.TryGetValue(ident.ToText(), out var obj))
                return obj;

            return null;
        }

        private object GetNative(string ident)
        {
            var ty = GetType();

            var field = ty.GetField(ident);
            if (field != null)
                return field.GetValue(this);

            var prop = ty.GetProperty(ident);
            if (prop != null)
                return prop.GetValue(this);

            var method = ty.GetMethod(ident);
            if (method != null)
                return method.GetMethodBody();

            return null;
        }

        public object Resolve(IPathname path)
        {
            throw new NotImplementedException();
        }
    }
}
