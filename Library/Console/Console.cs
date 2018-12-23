using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diver
{
    public class Console
    {
        public string Path => "/";
        public string Prompt => ">";

        public void Process(string input)
        {
        }

        private IRegistry _registry;
    }
}
