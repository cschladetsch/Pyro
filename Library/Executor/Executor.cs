using System.Collections.Generic;

namespace Diver.Executor
{
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
