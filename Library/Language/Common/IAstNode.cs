﻿namespace Pyro.Language
{
    using System.Collections.Generic;

    /// <summary>
    /// A node in a general Ast (Abstract Syntax Tree).
    /// </summary>
    public interface IAstNode<T>
    {
        IList<T> Children { get; }
    }
}

