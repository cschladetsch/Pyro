# Rho Language

This is a Python-like, more human-readable language to be used across the wire.

*Rho* translates to *Pi* and uses the exact same [Executor](/Language/Core/Executor).

## Coroutines

Has built-in suppor for Continuations, including the following three main operations:

* Suspend. This is similar to a traditional function call. Execution will return to current context after being suspended to the given coroutine.
* Replace. This will continue what is on the data stack, and will not return to current context.
* Resume. This drops the current context completely, and resumes whatever is on the data stack.

## Tests

See the [tests](/Test/TestRho)

## Examples

See the [tests](/Test/TestRho) and the [test scripts](/Test/TestRho/Scripts)
.