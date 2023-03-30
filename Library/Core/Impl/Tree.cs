using System;
using System.Collections.Generic;

namespace Pyro.Impl {
    internal class Tree
        : ITree {
        public IConstRefBase Root { get; set; }
        public IConstRefBase Scope { get; set; }
        public IConstRefBase Parent { get; }
        public IList<IConstRefBase> SearchPath { get; set; }

        public IConstRefBase Find(IPathname path) {
            return null;
        }

        public IEnumerable<IConstRefBase> GetChildren(IPathname path) {
            //if (path.IsAbsolute) {
            //    var node = Root;
            //    foreach (var element in path.Elements) {
            //        if (element.ElementType == PathElement.EType.Separator)
            //            continue;

            //        if (element.ElementType == PathElement.EType.Identifier) {
            //            if (node is IConstRefBase cr)
            //                node = cr.Scope[element.Identifer.ToText()] as IConstRefBase;
            //            else
            //                node = null;
            //        }
            //    }

            //    return node;
            //}
            return default;
        }

        public object Resolve(IIdentifer ident) {
            if (!(Scope is IConstRefBase cr)) {
                return GetNative(ident.ToText());
            }

            if (cr.Scope.TryGetValue(ident.ToText(), out var obj)) {
                return obj;
            }

            return null;
        }

        public object Resolve(IPathname path) {
            throw new NotImplementedException();
        }

        private object GetNative(string ident) {
            var ty = GetType();

            var field = ty.GetField(ident);
            if (field != null) {
                return field.GetValue(this);
            }

            var prop = ty.GetProperty(ident);
            if (prop != null) {
                return prop.GetValue(this);
            }

            var method = ty.GetMethod(ident);
            if (method != null) {
                return method.GetMethodBody();
            }

            return null;
        }
    }
}