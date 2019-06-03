# Rho Tests

Rho is an in-fix language that looks a lot like Python.

* *Rho* Translates to *Pi* code and is run on the same type of `Executor`.
* Can mix Pi and Rho code. They share the same Executor.
* Supports native continuations and parameterised functions.
* *Rho* is much more human-readable than *Pi*
* *Rho* takes longer to parse and execute. Think of Pi like asm and Rho like C/C++ (but with only duck typing)
    * That said, translating and executing many pages of *Rho* script from scratc takes ~0ms. 
    * But *Pi* will always be ~5x faster to translate and execute.
