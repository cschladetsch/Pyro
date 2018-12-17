using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver.Core.Impl
{
    public class ObjectBase : IObjectBase
    {
        public Id Id { get; }
        public IRegistry Registry { get; }
        public object Value { get; set; }
    }
}
