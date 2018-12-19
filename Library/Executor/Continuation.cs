using System.Collections.Generic;
using System.Reflection.Emit;

namespace Diver.Executor
{
    public class Continuation
    {
        public Continuation(IRef<List<object>> code)
        {
            _code = code.Value;
        }

        private List<object> _code = new List<object>();
        private Dictionary<Label, object> _scope = new Dictionary<Label, object>();
    }
}