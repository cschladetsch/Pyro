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

## Description

This is a C# program that creates a REPL (Read-Eval-Print Loop) console for the Pyro programming language. The console can connect to and enter into other consoles.

Here is a brief overview of the code:

Namespace imports: Various namespaces are imported for execution context, language, and networking.
Program class: This class inherits from AppCommon.AppCommonBase.
Constants and variables: ListenPort is the port number the console listens on for incoming connections. _peer is an instance of IPeer for connecting to other peers. _context is an instance of ExecutionContext for translation.
Main method: The entry point of the console application. It creates a new Program instance and calls the Repl method.
Program constructor: Initializes the execution context, starts a peer, sets up the peer, and runs initialization scripts.
SetupPeer method: Sets up event handlers for the _peer.
StartPeer method: Starts the peer, sets up the local execution context, and registers classes.
RunInitialisationScripts method: Placeholder for running initialization scripts like ~/.pyrorc.{pi,rho}.
Repl method: Main loop of the console that reads input, executes it, and writes the data stack.
WritePrompt method: Writes the prompt to the console.
GetInput method: Reads input from the user.
PreProcess method: Handles special input commands for help, language switching, and leaving the current context.
Execute method: Translates and executes the input in the current context.
OnConnected method: Event handler called when connected to a client.
WriteDataStackContents method: Writes the contents of the data stack to the console.
WriteDataStack method: Writes the data stack of the current context to the console.
ShowHelp method: Displays help information about the console.
Shutdown method: Shuts down the console, stops the peer, and exits the program.
In summary, this is a console application for the Pyro language that allows users to enter code, execute it in the local or remote context, and display the results.

## TODO

* Replicate remote data-stack.
* Allow for remote object references.



