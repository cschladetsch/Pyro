using System;

namespace Diver
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
        public override string ToString()
        {
            return "Context stack empty";
        }
    }

    public class DebugBreakException : Exception
    {
        public override string ToString()
        {
            return "DebugBreak";
        }
    }

    public class NullValueException : Exception
    {
        public override string ToString()
        {
            return "Unexpected Null";
        }
    }

    public class AssertionFailedException : Exception
    {
        public override string ToString()
        {
            return "Assertion Failed";
        }
    }

    public class DataStackEmptyException : Exception
    {
        public override string ToString()
        {
            return "Empty stack";
        }
    }

    public class UnknownIdentifierException : Exception
    {
        public object What;

        public UnknownIdentifierException(object obj)
        {
            What = obj;
        }

        public override string ToString()
        {
            return $"Unknown object '{What}'";
        }
    }

    public class CannotCompareEnumerationsException : Exception
    {
        public object Left, Right;
        public CannotCompareEnumerationsException(object o, object o1)
        {
            Left = o;
            Right = o1;
        }

        public override string ToString()
        {
            return $"Cannot enumerate {Left.GetType().Name} with {Right.GetType().Name}";
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

        public TypeMismatchError(Type expected, Type got) : base()
        {
            Expected = expected;
            Got = got;
        }

        public override string ToString()
        {
            return $"Expected {Expected}, got {Got}";
        }
    }
}
