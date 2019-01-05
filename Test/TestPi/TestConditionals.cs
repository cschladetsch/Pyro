using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestConditionals : TestCommon
    {
        [Test]
        public void TestIfElse()
        {
            PiRun("1 true if");
            AssertPop(1);

            PiRun("1 false if");
            AssertEmpty();

            PiRun("1 2 true ife");
            AssertPop(1);

            PiRun("1 2 false ife");
            AssertPop(2);
        }
    }
}