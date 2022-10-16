namespace Pyro.Test {
    using NUnit.Framework;

    /// <inheritdoc />
    /// <summary>
    /// Test classes in Rho-space.
    /// </summary>
    [TestFixture()]
    internal class TestRhoClass
        : TestCommon {
        [Test]
        public void TestBasicClass() {
            RunScript("BasicClass.rho");
        }
    }
}

