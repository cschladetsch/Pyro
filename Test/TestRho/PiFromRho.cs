using NUnit.Framework;

namespace Pyro.Test
{
    /// <inheritdoc />
    /// <summary>
    /// Run pi from rho.
    /// </summary>
    [TestFixture()]
    public class PiFromRho
        : TestCommon
    {
        [Test]
        public void TestBasic()
        {
            RhoRun("`1`");
            Assert.AreEqual(1, Pop());

            RhoRun("`1 2 +`");
            Assert.AreEqual(3, Pop());

            Assert.AreEqual(_exec.ContextStack.Count, 0);
            Assert.AreEqual(_exec.DataStack.Count, 0);
        }

        [Test]
        public void RunScripts()
        {
            RunScript("pyro/PiFromRho0.rho");
            RunScript("pyro/PiFromRho1.rho");
            RunScript("pyro/PiFromRho2.rho");
        }
    }
}

