using NUnit.Framework;

namespace Pyro.TestPi
{
    using Test;
    using Language.Lexer;

    /// <inheritdoc />
    /// <summary>
    /// Test floating-point tokenising, parsing and arithmetic.
    /// </summary>
    [TestFixture]
    public class TestFloats
        : TestCommon
    {
        [Test]
        public void TestFloatOps()
        {
            AssertSameTokens("1.1", EPiToken.Float);
            AssertSameTokens("1.1 1.2 +", EPiToken.Float, EPiToken.Float, EPiToken.Plus);

            PiRun("1.1 2.2 +");
            Assert.AreEqual(1.1f + 2.2f, Pop<float>());

            PiRun("1.1 2.2 *");
            Assert.AreEqual(1.1f * 2.2f, Pop<float>());

            PiRun("1.1 2.2 3.54234 * +");
            Assert.AreEqual(3.54234f*2.2f + 1.1f, Pop<float>());

            PiRun("1.1 2.2 -");
            Assert.AreEqual(1.1f - 2.2f, Pop<float>());

            PiRun("1.1 2.2 div");
            Assert.AreEqual(1.1f/2.2f, Pop<float>());
        }
    }
}

