# Pi Language

This is a post-fix language that is very fast to parse and execute, but is not very easy for a humman to write complex programs with.

## Two Stacks

Most language systems have one exposed stack: the data stack. This is used to keep track of the history of contexts that led to the current context, and is also used to store local variables.

In Pi, there are two distinct stacks that can be manipulated directly and separately:

* The DataStack
* The ContextStack

## Coroutines

Has built-in suppor for Continuations, including the following three main operations:

* Suspend. This is similar to a traditional function call. Execution will return to current context after being suspended to the given coroutine.
* Replace. This will continue what is on the data stack, and will not return to current context.
* Resume. This drops the current context completely, and resumes whatever is on the data stack.

## Tests

See the [tests](/Test/TestPi)

## Examples

See the tests.
