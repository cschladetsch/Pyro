using System;
using System.Collections.Generic;

namespace Pyro.BuiltinTypes {
    public class CustomMapComparer
        : IEqualityComparer<object> {
        public new bool Equals(object x, object y) {
            throw new NotImplementedException();
        }

        public int GetHashCode(object obj) {
            throw new NotImplementedException();
        }
    }
}