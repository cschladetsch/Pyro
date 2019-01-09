# Common Language Library

This assembly should be pulled out into its own repository so it can be used generally to make custom DSLs in .NET without all the hassle of third-party tools like ANTLR etc.

To be clear: this library hsas no other requirements other than Diver.Core and even that could be removed for everything but the Translator.

Contains functionality and data common to all supported custom languages:

* [Pi](../PiLang/Readme.md)
* [Rho](../RhoLang/Readme.md)
* [Tau](../TauLang/Readme.md) TBD

This commonality includes:

* Common [Process](Process.cs) system with error tracking.
* Common [Lexer](LexerCommon.cs) and [Parser](ParserCommon.cs), parameterised over enum and node types.
* Common generation of lexer, parser and translator error messsages.

