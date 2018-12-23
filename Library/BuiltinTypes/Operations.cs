using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuiltinTypes
{
    public static class Operations
    {
        public static bool Equals(object A, object B)
        {
            if (A == null)
                return B == null;
            if (B == null)
                return false;

            if (A is IRefBase a)
            {

            }
        }
    }
}
