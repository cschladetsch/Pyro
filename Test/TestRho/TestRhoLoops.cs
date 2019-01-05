using NUnit.Framework;

namespace Diver.Test.Rho
{
    [TestFixture]
    public class TestRhoLoops : TestCommon
    {
        [Test]
        public void TestForEachIn()
        {
            RhoRun(@"
c = 0
for (a in [1, 2, 3])
	writeln(a)
	c = c + a
assert(c == 1 + 2 + 3)
", true);
            
        }
        
    }
}