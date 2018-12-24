﻿using System.Collections.Generic;

namespace Diver.Language
{
    public interface IAstNode<T>
    {
        IList<T> Children { get; }
    }
}