using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection.Emit;
using Diver;

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

    public class Executor
    {
        public List<object> Data => _data;
        public List<object> Context => _context;

        void Continue(Continuation cont)
        {
        }

        private List<object> _data = new List<object>();
        private List<object> _context = new List<object>();
    }
}
