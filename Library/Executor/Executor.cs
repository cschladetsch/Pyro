using System.Collections.Generic;

namespace Diver.Executor
{
    public enum EOperation
    {
        Plus,
        Minus,
        Multiply,
        Divide,

        Store,
        Retrieve,

        Suspend,
        Resume,
        Replace,
        
        Print,
        Assert,

        If,
        IfThen,
        IfThenElse,

        ToArray,
        ToMap,
        ToSet,
        ToPair,

        Expand,
    }

    public class Executor
    {
        public List<object> Data => _data;
        public List<object> Context => _context;

        void Continue(Continuation cont)
        {
            object next;
            while (cont.Next(out next))
            {
                if (next is IRefBase obj)
                {
                }
            }
        }


        private List<object> _data = new List<object>();
        private List<object> _context = new List<object>();
    }
}
