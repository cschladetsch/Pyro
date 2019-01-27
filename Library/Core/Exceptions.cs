using System;

namespace Pryo
{
    public class MemberNotFoundException : Exception
    {
        public MemberNotFoundException(Type type, string member)
            : base($"Member {member} not found in {type.Name}")
        {
        }

        public MemberNotFoundException(string typeName, string member)
            : base($"Member {member} not found in {typeName}")
        {
        }
    }

    public class CouldNotMakeClass : Exception
    {
        public CouldNotMakeClass(Type type)
            : base($"Couldn't make class for {type.FullName}")
        {
        }
    }

    public class ContextStackEmptyException : Exception
    {
        public ContextStackEmptyException() : base("Empty ContextStack")
        {
        }
    }

    public class DebugBreakException : Exception
    {
        public DebugBreakException() : base("DebugBreak")
        {
        }
    }

    public class NullValueException : Exception
    {
        public NullValueException() : base("Null value")
        {
        }
    }

    public class AssertionFailedException : Exception
    {
        public AssertionFailedException() : base("Assertion failed")
        {
        }
    }

    public class DataStackEmptyException : Exception
    {
        public DataStackEmptyException() : base("Empty Stack")
        {
        }
    }

    public class UnknownIdentifierException : Exception
    {
        public object What;
        public UnknownIdentifierException(object obj) : base($"Unknown object '{obj}'")
        {
            What = obj;
        }
    }

    public class CannotCompareEnumerationsException : Exception
    {
        public object Left, Right;
        public CannotCompareEnumerationsException(object left, object right)
            : base( $"Cannot enumerate {left.GetType().Name} with {right.GetType().Name}")
        {
            Left = left;
            Right = right;
        }
    }

    public class CannotResolve : Exception
    {
        public CannotResolve(string ident) : base($"Couldn't resolve {ident}")
        {
        }
    }

    public class TypeMismatchError : Exception
    {
        public Type Expected, Got;

        public TypeMismatchError(Type expected, Type got) : base($"Expected {expected}, got {got}")
        {
            Expected = expected;
            Got = got;
        }
    }
}
