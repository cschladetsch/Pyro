# Pyro :sun_with_face:

*Pyro* provides a framework for distrubuted, interactive virtual reality experiences.

## Overview

*Pyro* is a cross-platform collection of .Net libraries supporting object persistence, networking, and a coroutine micro-kernel.

Each major component has its own project, and its own `Readme.md` file.

*Pyro* is based on two language systems, an object registry, and a generalised virtual machine named *Executor*.

The two key languages are:

* **Pi**. A reverse-polish double stack-based language with fast lexing and parsing.
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

Each project also copies its output assembly to the `Liminal/Assemblies` folder within the sample Unity3d project.
