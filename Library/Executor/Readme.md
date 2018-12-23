# Executor

This is a virtual machine with two distinct stacks:

* Data stack. This is familiar and holds arguments and results.
* Context stack. This is a stack of Continuations that can be directly modified.

The executor is quite simple in C# compared to writing one in C++.