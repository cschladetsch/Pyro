using System;
using System.Diagnostics.CodeAnalysis;

namespace Pyro {
    /// <inheritdoc />
    /// <summary>
    ///     Given member (field, property, method, inner class etc) not found.
    /// </summary>
    public class MemberNotFoundException
        : Exception {
        public MemberNotFoundException(Type type, string member)
            : base($"Member {member} not found in {type.Name}") {
        }

        public MemberNotFoundException(string typeName, string member)
            : base($"Member {member} not found in {typeName}") {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Couldn't find class to make in Registry.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.Csharp.Maintainability", "*")]
    public class CouldNotMakeClass
        : Exception {
        public CouldNotMakeClass(Type type)
            : base($"Couldn't make class for {type.FullName}") {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     The context stack was empty, and yet was attempted to be popp'ed
    /// </summary>
    public class ContextStackEmptyException
        : Exception {
        public ContextStackEmptyException()
            : base("Empty ContextStack") {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Used for debugging.
    /// </summary>
    public class DebugBreakException
        : Exception {
        public DebugBreakException()
            : base("DebugBreak") {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Thrown when an unexpected null reference was found
    /// </summary>
    public class NullValueException
        : Exception {
        public NullValueException(string text = "Null value")
            : base(text) {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     An Assertion failed.
    /// </summary>
    public class AssertionFailedException
        : Exception {
        public AssertionFailedException()
            : base("Assertion failed") {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     An attempt was made to access the contents of and empty data-stack in an Executor.
    /// </summary>
    public class DataStackEmptyException
        : Exception {
        public DataStackEmptyException(string text = "Empty Stack")
            : base(text) {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Given identifier could not be resolved locally, in the context stack, or in the tree.
    /// </summary>
    public class UnknownIdentifierException
        : Exception {
        public object What;

        public UnknownIdentifierException(object obj)
            : base($"Unknown object '{obj}'") {
            What = obj;
        }
    }

    public class NotEnoughArgumentsException
        : Exception {
        public NotEnoughArgumentsException(string text)
            : base(text) {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Invalid comparisons between two different enumeration types.
    /// </summary>
    public class CannotCompareEnumerationsException
        : Exception {
        public object Left, Right;

        public CannotCompareEnumerationsException(object left, object right)
            : base($"Cannot enumerate {left.GetType().Name} with {right.GetType().Name}") {
            Left = left;
            Right = right;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Cannot resolve given identifier.
    /// </summary>
    public class CannotResolve
        : Exception {
        public CannotResolve(string ident)
            : base($"Couldn't resolve {ident}") {
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     There was a mis-match in types for a given binary operation.
    /// </summary>
    public class TypeMismatchError
        : Exception {
        public Type Expected, Got;

        public TypeMismatchError(Type expected, Type got)
            : base($"Expected {expected}, got {got}") {
            Expected = expected;
            Got = got;
        }
    }
}