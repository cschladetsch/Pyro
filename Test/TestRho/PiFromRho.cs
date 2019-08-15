namespace Pyro.Test
{
    using NUnit.Framework;
    using Language;

    /// <inheritdoc />
    /// <summary>
    /// Run pi from rho.
    /// </summary>
    [TestFixture]
    public class PiFromRho
        : TestCommon
    {
        [Test]
        public void TestBasic()
        {
            RhoRun("`1`", false, EStructure.Expression);
            Assert.AreEqual(1, Pop());

            RhoRun("`1 2 +`");
            Assert.AreEqual(3, Pop());

            Assert.AreEqual(_Exec.ContextStack.Count, 0);
            Assert.AreEqual(_Exec.DataStack.Count, 0);

            RhoRun("assert(`1 1 +` == 2)");
            RhoRun("assert(`1 2 {+} &` == 3)");
        }

        [Test]
        public void RunScripts()
        {
            RunScript("PiRho/PiFromRho0.rho");
            RunScript("PiRho/PiFromRho1.rho");
        }
    }
}

