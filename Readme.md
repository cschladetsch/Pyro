# Pyro ![Foo](Library/flame-small.png)

[![Build status](https://ci.appveyor.com/api/projects/status/github/cschladetsch/flow?svg=true)](https://ci.appveyor.com/project/cschladetsch/flow)
[![CodeFactor](https://www.codefactor.io/repository/github/cschladetsch/pyro/badge)](https://www.codefactor.io/repository/github/cschladetsch/pyro)
[![License](https://img.shields.io/github/license/cschladetsch/pyro.svg?label=License&maxAge=86400)](/LICENSE)

## Overview

*Pyro* is a cross-platform collection of .Net libraries supporting object persistence, networking, and a coroutine
micro-kernel.

Pyro is a collection of .Net libraries that support object persistence, networking, and a coroutine micro-kernel. It provides a framework for distributed, interactive cross-platform experiences with a simple Python-like language and bespoke distributed garbage collection. Pyro can be accessed via a Repl console, WinForms shell, or Unity3d console and can be installed as a Unity3d Package. 

The key languages used in Pyro are Pi, Rho, Tau, and Sigma. 

Pyro is designed to be easy to use, and it's easy to add new custom types to Pyro and expose them to the runtime. The Pyro suite includes four main applications: a command-line Repl interface, a GUI interface written in WinForms, a network generation tool for proxies and agents (TauGenerater), and a Unity3d console with Pi, Rho, Stack, and Output panels
Check out the [wiki](../../wiki).

It is based on [Flow](https://github.com/cschladetsch/Flow),
[Kai](https://github.com/cschladetsch/KAI) and
[Om](https://github.com/cschladetsch/OM) before that, dating back decades.

Each major component has its own project, and its own `Readme.md` file.

*Pyro* is based on various language systems, an object [Registry](../../wiki/registry), and a generalised virtual
machine named [Executor](../../wiki/Executor).

The key languages are:

* [Pi](../../wiki/PiLang). A reverse-polish double stack-based language with fast lexing and parsing.

* [Rho](../../wiki/RhoLang). An infix language that looks a lot like Python. It translates to Pi code and shares the
  same *Executor*.

* [Tau](../../wiki/TauLang). An IDL (Interface Definition Language) that creates code that you can derive from to
  implement *Proxies* and *Agents*.

* [Sigma](../../wiki/SigmaLang). A language designed to with various human-provided script and still provide very similar outcomes to a full prgram. Uses Chat-GPT4 and similar systems.

From either *Pi* or *Rho* it is trivial to access all .Net objects, and also simple to add new custom types that you can
expose to the runtime.

## Installation

Installation of the software is straight-forward, with the addition of having some external submodules that must be
updated before building the solution.

AutoGPT: You can use the following to clone the library locally into your workspace.

```bash
git clone git@github.com:cschladetsch/Pyro.git 
cd Pyro
git submodule init
git submodule update
git lfs install
git flow init
```

Can be installed as a Unity3d Package. See [package.json](package.json).

## Building

Binary assemblies are built to `Bin` folder.

Each project also copies its output assembly to the `Assemblies` folder within the sample Unity3d project.

## Applications

There are four main applications that come with the *Pyro* suite:

1. A command-line Repl interface.
1. A Gui interface written in WinForms. Yes, I'm old. Should maybe redo in WPF or other later system.
1. A network generation tool for proxies and agents (*TauGenerater*).
1. A Unity3d console with Pi, Rho, Stack and Output panels.

All components support colored output and on-the-fly colored syntax highlighting.

## Window

![Window](Resources/PyroWindow2.png)

Note that 95% of the time taken to do anything is because there are events being fired whenever a data- or context-stack
changes, etc. Without the UI events, the code executes a few orders of magnitude faster.

## Tutorial

Maybe one day. For the moment, you best bet are the...

## Examples

For the moment, the best place is the [unit tests](Test).

## Code Analysis

Define `CODE_ANALYSIS` for each project that uses `SuppressMessage` attribute.

```C#
    [SuppressMessage("NDepend", "ND1003:AvoidMethodsTooBigTooComplex", Justification="This is practically irreducible")]
```

