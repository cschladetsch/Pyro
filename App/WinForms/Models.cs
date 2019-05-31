using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinForms
{
    class UserModel
    {
        public string Email { get; set; }
        private Orgnisation Organisation { get; set; }
    }

    class Orgnisation
    {
        public string Name { get; set; }
    }


}

