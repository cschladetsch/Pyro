using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestConditionals : TestCommon
    {
        [Test]
        public void TestIfElse()
        {
            Run("1 true if");
            AssertPop(1);

            Run("1 false if");
            AssertEmpty();

            Run("1 2 true ife");
            AssertPop(1);

            Run("1 2 false ife");
            AssertPop(2);

        }
    }
}