using System.Collections.Generic;

namespace Pyro.Language {
    /// <summary>
    ///     A node in a general Ast (Abstract Syntax Tree).
    /// </summary>
    public interface IAstNode<T> {
        IList<T> Children { get; }
    }
}