# Console

This is a **repl** (read-execute-print-loop) cross-platform text-based console for the system.

It can connect to and remotely execute code on other peers in the domain. It can also be connected to by any other node. There is currently no security or ideas of users or permissions.

There are two main languages used: Pi and Rho. You can switch between them by typing `pi` or `rho` at the prompt.

Whatever you enter at the prompt is sent to and executed in the context shown by the prompt. By default, this is the local machine.

Typing `help` or `?` provides a simple help message.

Press control-C to quit.

## Commands

There isn't really a 'list of commands'. The language used exposes the .Net runtime so you can access pretty much everything.

The syntax of pi and rho are explained in their relative locations.

## TODO

* Replicate remote data-stack.
* Allow for remote object references.



