# Language

This folder contains everything pertaining to Languages in the _Pyro_ system.

There are three main languages:

* [_Pi_](https://github.com/cschladetsch/Pyro/wiki/PiLang) a reverse polish notation language with an associated Executor and namespace/variable Tree
* [_Rho_](https://github.com/cschladetsch/Pyro/wiki/RhoLang) an in-fix language that reads a lot like Python. It it is Translated to Pi and executed in the same way.
* [_Tau_](https://github.com/cschladetsch/Pyro/wiki/TauLang) an interface definition language (IDL) that is used to to make [Proxies](../../Proxies) and [Agents](../../wiki/Agents) to be used by the Networking layer.

All of these languages use the same templated `Lexer<>`, `Parser<>` and `Translater<>`. This means that each language is about a few hundred lines long.

Work has start on a new language called [_Sigma_](https://github.com/cschladetsch/Pyro/wiki/SigmaKang) intended to directly interact with external [Artificial Intelligence] systems and code-generation.
