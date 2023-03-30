using System.Collections.Generic;

namespace Pyro {
    /// <summary>
    ///     A hierarchy of objects with a search path.
    /// </summary>
    public interface ITree {
        /// <summary>
        ///     The current node in the tree.
        /// </summary>
        IConstRefBase Scope { get; set; }

        /// <summary>
        ///     The parent of the current node, if any.
        /// </summary>
        IConstRefBase Parent { get; }

        /// <summary>
        ///     Search path for identifiers not found in local scope.
        /// </summary>
        IList<IConstRefBase> SearchPath { get; set; }

        IConstRefBase Root { get; }

        IConstRefBase Find(IPathname path);

        IEnumerable<IConstRefBase> GetChildren(IPathname path);

        object Resolve(IIdentifer ident);

        object Resolve(IPathname path);
    }
}