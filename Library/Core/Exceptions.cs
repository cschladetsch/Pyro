using System;

namespace Diver
{
    public class MemberNotFoundException : Exception
    {
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
    }

    public class DebugBreakException : Exception
    {
    }

    public class NullValueException : Exception
    {
    }

    public class DataStackEmptyException : Exception
    {
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
