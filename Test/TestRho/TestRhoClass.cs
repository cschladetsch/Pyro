using NUnit.Framework;

namespace Pyro.Test
{
    /// <inheritdoc />
    /// <summary>
    /// Test classes in Rho-space.
    /// </summary>
    [TestFixture()]
    class TestRhoClass
        : TestCommon
    {
        [Test]
        public void TestBasicClass()
        {
            RunScript("BasicClass.rho");
        }
    }
}

