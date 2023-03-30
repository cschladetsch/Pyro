# Executor

A virtual machine with two distinct stacks:

* `Data stack`. This is familiar and holds arguments for and results of *operations*
* `Context stack`. This is a stack of *Continuations*.

## Overview

This C# code defines a class Executor inside the namespace Pyro.Exec. The Executor class is responsible for processing a
sequence of Continuation objects, which represent units of execution in the Pyro language runtime.

Key elements of the Executor class include:

Properties:

* DataStack: A stack of data for the executor to process.
* ContextStack: A list of Continuation objects that represent the current execution context.
* Rethrows: A boolean that indicates whether exceptions should be rethrown or not.
* Current: The current Continuation object in the execution process.
* SourceFilename: A string containing the source filename of the code being executed.

### Events

* OnContextStackChanged: Triggered when the context stack changes.
* OnDataStackChanged: Triggered when the data stack changes.
* OnContinuationChanged: Triggered when the current continuation changes.

### Constructor

Executor(): Initializes a new Executor instance, setting up the initial state and adding operations.
Methods for managing continuations and execution:

* PushContext(): Pushes a new Continuation onto the context stack.
* Continue(): Continues execution of the current or provided Continuation.
* Single(): Executes the next step in the current Continuation.
* Next(): Advances to the next operation in the current Continuation.
* Prev(): Not implemented, intended for moving to the previous operation.
* Perform(): Performs an operation based on the input object.

Methods for managing the data stack:

* Push(): Pushes an object onto the data stack.
* Pop(): Pops an object from the data stack, with support for dynamic typing.
* Pop<T>(): Pops an object of type T from the data stack.
* DataStackPush(): Internal method to push an object onto the data stack and fire the event.
* DataStackPop(): Internal method to pop an object from the data stack and fire the event.

Methods for resolving identifiers and handling execution:

* TryResolve(): Tries to resolve an object or identifier to a value.
* TryResolvePath(): Not implemented, intended for resolving a path to a value.
* TryResolveContextually(): Tries to resolve a label by checking the context stack and scope.
* Suspend(): Suspends the current continuation and resumes the parent continuation.
* Resume(): Resumes the continuation that spawned the current one, handling various types of callable objects and
  methods.
* Break(): Stops the current continuation and resumes whatever is on the context stack.

The Executor class is designed to manage the execution of Pyro language programs by managing the data stack and context
stack, resolving identifiers, and handling continuations. It is an essential component of the Pyro language runtime.
