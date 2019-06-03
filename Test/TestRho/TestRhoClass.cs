using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Pyro.Test;

namespace TestRho
{
    [TestFixture()]
    class TestRhoClass
        : TestCommon
    {
        [Test]
        public void TestBasicClass()
        {
            RunScript("Class.rho");
        }
    }
}

