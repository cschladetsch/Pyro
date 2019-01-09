# Pyro

Pyro provides a framework for distrubuted, interactive virtual reality experiences.

## Overview

*Pyro* is a cross-platform .Net library supporting object persistence, networking, and a coroutine micro-kernel.

Each major component has its own project, and its own Readme.md file.

It is based on two language systems, an object registry, and generalised virtual machine or Executor.

The two languages are:

* **Pi**. A reverse-polish stack-based language with fast lexing and parsing.
* **Rho**. An infix language that looks a lot like Python. It translates to Pi code.

From *Pi* or *Rho* it is trivial to access all .Net objects, and also simple to add new custom types.

## Development Environment Setup

* Install [GitBash](https://gitforwindows.org/)
* Install [Visual Studio Community Edition](https://www.google.com/&q=visual%32studio%32community)
* Install [Unity3d](https://www.unity3d.com)
* Install the [JDK](https://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html).
* Install [Android Studio](https://dl.google.com/dl/android/studio/install/3.2.1.0/android-studio-ide-181.5056338-windows.exe)
* Install [Android Native SDK](https://dl.google.com/android/repository/android-ndk-r18b-windows-x86_64.zip)

## Software Installation

Installation of the software is straight-forward, with the addition of having some external submpdules that must be updated before building the solution.

```bash
$ git clone git@github.com:cschladetsch/Pyro.git
$ git submodule init
$ git submodule update
```

## Build the Libraries

Binary assemblies are built to `Bin` folder. 

Each project also copies its output assembly to the `Liminal/Assemblies` folder within the sample Unity3d project.
