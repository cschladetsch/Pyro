# Common Language Library

This assembly should be pulled out into its own repository so it can be used generally to make custom DSLs in .NET without all the hassle of third-party tools like ANTLR etc.

Contains functionality and data common to all supported custom languages:

* [Pi](../PiLang/Readme.md)
* [Rho](../RhoLang/Readme.md)
* Tau. TODO.

This commonality includes:

* Common [Process](Process.cs) system with error tracking.
* Common Lexer and Parser, parameterised over enum and node types.

