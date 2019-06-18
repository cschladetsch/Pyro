using System.Collections.Generic;

namespace Pyro.Language
{
    public interface IAstNode<T>
    {
        IList<T> Children { get; }
    }
}
