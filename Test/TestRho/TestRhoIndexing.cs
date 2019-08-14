namespace Pyro.Test.Rho
{
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <inheritdoc />
    /// <summary>
    /// Test basic Rho functionality: boolean logic, arithmetic, strings, conditionals.
    /// </summary>
    [TestFixture]
    public class TestRhoIndexing
        : TestCommon
    {
        [Test]
        public void Test()
        {
            var a = new List<int> {1, 2, 3};
            DataStack.Push(a);
            RhoRun(
@"
a = {1 2 3}
a[1]
");
            Assert.AreEqual(2, Pop<int>());
        }
    }
}

