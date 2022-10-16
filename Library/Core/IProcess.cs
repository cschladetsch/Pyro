﻿namespace Pyro
{
    /// <summary>
    /// A subtle class that adds rich semantics and usage patterns
    /// to derived types.
    ///
    /// Notably each language's Lexer, Parser, and Translator are all Proceses's.
    ///
    /// The base of all netork classes are also Process's.
    ///
    /// This is the work-horse of many systems. A very humble yet very important and used class.
    /// </summary>
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}

