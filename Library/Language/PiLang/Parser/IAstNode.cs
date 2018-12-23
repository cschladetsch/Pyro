using System.Collections.Generic;

namespace Diver.Language.PiLang
{
    public interface IAstNode<T>
    {
        IList<T> Children { get; }
    }
}