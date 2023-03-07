# Common Language Library

Contains functionality and data common to all supported custom languages:

* [Pi](../PiLang/Readme.md)
* [Rho](../RhoLang/Readme.md)
* [Tau](../TauLang/Readme.md) TBD

This commonality includes:

* Common [Process](Process.cs) system with error tracking.
* Common [Lexer](LexerCommon.cs) and [Parser](ParserCommon.cs), parameterised over enum and node types.
* Common generation of Lexer, Parser and Translator error messages.

## Todo

There could be even more refactoring to make the deduced languages use less code.

This is a tricky business, as it comes at a higher degree of complexity and a lower degree of maintainance.
