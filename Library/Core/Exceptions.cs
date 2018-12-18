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
}
