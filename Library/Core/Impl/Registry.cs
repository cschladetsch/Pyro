using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver.Core.Impl
{
    public class Registry
    {
        public Class<T> Register<T>()
        {
            return null;
        }

        private Dictionary<Guid, ClassBase> _classes = new Dictionary<Guid, ClassBase>();
        private Dictionary<Id, ObjectBase> _instances = new Dictionary<Id, ObjectBase>();
    }
}
