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
}
