using NUnit.Framework;
using Pyro.Language.Lexer;

namespace Pyro.TestPi
{
    using Test;

    /// <summary>
    /// Test floating-point tokenising, parsing and arithmetic.
    /// </summary>
    [TestFixture]
    public class TestFloats
        : TestCommon
    {
        [Test]
        public void TestFloatAdd()
        {
            AssertSameTokens("1.1", EPiToken.Float);
            AssertSameTokens("1.1 1.2 +", EPiToken.Float, EPiToken.Float, EPiToken.Plus);
            PiRun("1.1 2.2 +");
            Assert.AreEqual(1.1f + 2.2f, Pop<float>());
        }
    }
}

