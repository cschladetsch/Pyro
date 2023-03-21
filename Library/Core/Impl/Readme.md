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

## IReflected and IReflected<<T>>

This is a C# code for two interfaces called "IReflected" and "IReflected<T>". These interfaces are used to provide a way for instances of classes to have access to their own reference base, which is an instance of the "IRefBase" interface.

The "IReflected" interface declares a single property called "SelfBase" of type "IRefBase". This property provides a way for instances of classes that implement this interface to access their own reference base.

The "IReflected<T>" interface is a generic interface that extends the "IReflected" interface. It declares a single property called "Self" of type "IRef<T>". This property provides a way for instances of classes that implement this interface to access their own typed reference, which is an instance of the "IRef<T>" interface.

Both interfaces are used in the Pyro system to provide instances of classes with access to their own reference base or typed reference. This allows instances of classes to be managed more easily and efficiently within the system.
 
## IClassBase and IClass<<T>>

This is a C# code for a class called "Class<T>" that extends the "ClassBase" class and implements the "IClass<T>" interface. This class is used to represent a specific class within the Pyro system and provides methods for creating new instances of the class, creating new references to the class, and converting instances of the class to PiScript.

The class contains a constructor that takes an instance of the "IRegistry" interface and an action that represents a method for converting instances of the class to text. The "NewInstance" method creates a new instance of the class and adds reference fields to it. The "NewRef" method creates a new reference to the class with the specified identifier and value. The "CreateConst" method creates a new constant reference to the class with the specified identifier and value.

The "ToPiScript" method and "AppendText" method are used to convert instances of the class to PiScript. The "AppendText" method takes a string builder and an instance of the class and appends text representing the instance to the string builder. If an action for converting instances of the class to text was provided during construction, it will be used; otherwise, the default implementation converts the instance to a string and appends it to the string builder.

Overall, the "Class<T>" class is used to represent specific classes within the Pyro system and provides methods for creating, managing, and converting instances of those classes.
 
## Tests
See [Core.Tests](https://github.cschladetsch/Pyro/Tests/Core).

