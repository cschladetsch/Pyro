# Core.Impl
Implementation of the Core library for *Pyro*.

You can think of this as being equivalent to the `System` dll for .Net.

Contains definitions for, among other things:
* IRegistry
* IObject
* IClass
* ITree
* ICallable
* IReflected
* Identifier
* Process

## IReflected and IReflected<T>

This is a C# code for two interfaces called "IReflected" and "IReflected<T>". These interfaces are used to provide a way for instances of classes to have access to their own reference base, which is an instance of the "IRefBase" interface.

The "IReflected" interface declares a single property called "SelfBase" of type "IRefBase". This property provides a way for instances of classes that implement this interface to access their own reference base.

The "IReflected<T>" interface is a generic interface that extends the "IReflected" interface. It declares a single property called "Self" of type "IRef<T>". This property provides a way for instances of classes that implement this interface to access their own typed reference, which is an instance of the "IRef<T>" interface.

Both interfaces are used in the Pyro system to provide instances of classes with access to their own reference base or typed reference. This allows instances of classes to be managed more easily and efficiently within the system.
 
## Tests
See [Core.Tests](https://github.cschladetsch/Pyro/Tests/Core).

