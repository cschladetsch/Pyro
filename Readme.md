# Pyro ![Foo](Library/flame-small.png)
[![Build status](https://ci.appveyor.com/api/projects/status/github/cschladetsch/pyro?svg=true)](https://ci.appveyor.com/project/cschladetsch/pyro)
[![CodeFactor](https://www.codefactor.io/repository/github/cschladetsch/pyro/badge)](https://www.codefactor.io/repository/github/cschladetsch/pyro)
[![License](https://img.shields.io/github/license/cschladetsch/pyro.svg?label=License&maxAge=86400)](./LICENSE.txt)
[![Release](https://img.shields.io/github/release/cschladetsch/pyro.svg?label=Release&maxAge=60)](https://github.com/cschladetsch/pyro/releases/latest)

## Overview

**Note: The build is only failing because of issues with appveyor and submodules which this library uses. The actual code builds fine.**

*Pyro* is a cross-platform collection of .Net libraries supporting object persistence, networking, and a coroutine micro-kernel.

Each major component has its own project, and its own `Readme.md` file.

*Pyro* is based on two language systems, an object registry, and a generalised virtual machine named *Executor*.

The two key languages are:

* **Pi**. A reverse-polish double stack-based language with fast lexing and parsing. See [pi](https://github.com/cschladetsch/Pyro/wiki/Pi).

* **Rho**. An infix language that looks a lot like Python. It translates to Pi code and shares the same *Executor*.

From either *Pi* or *Rho* it is trivial to access all .Net objects, and also simple to add new custom types that you can expose to the runtime. 

## Installation

Installation of the software is straight-forward, with the addition of having some external submodules that must be updated before building the solution.

```bash
$ git clone git@github.com:cschladetsch/Pyro.git
$ git submodule init
$ git submodule update
$ git lfs install
$ git flow init
```

## Building

Binary assemblies are built to `Bin` folder. 

Each project also copies its output assembly to the `Assemblies` folder within the sample Unity3d project.

## Applications
There are four main applications that come with the *Pyro* suite:
1. A command-line Repl interface.
1. A Gui interface.
1. A network generation tool for proxies and agents.
1. A Unity3d console with Pi, Rho, Stack and Output panels.

All components support colored output and on-the-fly colored syntax highlighting.

## Code Analysis

Define `CODE_ANALYSIS` for each project that uses `SuppressMessage` attribute.

```C#
    [SuppressMessage("NDepend", "ND1003:AvoidMethodsTooBigTooComplex", Justification="This is practically irreducible")]
```

