using NUnit.Framework;
using NUnit.Framework.Constraints;

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
	c = c + a
assert(c == 1 + 2 + 3)
");

            RhoRun(@"
c = 0
for (a in [1,2])
	for (b in [4,5])
		c = c + b
	c = c + a
");
			WriteLine("-----");
	        var c = 0;
	        for (var a = 1; a < 3; ++a)
	        {
				WriteLine(a);
		        for (var b = 4; b < 6; ++b)
		        {
			        c += b;
			        //WriteLine(c);
                }
		        c += a;
				WriteLine(a);
				//WriteLine(c);
	        }
			AssertVarEquals("c", c);
        }

        //[Test]
        public void TestForEachLoop()
        {
            RhoRun(@"
c = 0
for (a = 0; a < 8; ++a)
	writeln(a)
	c = c + a
assert(c == 1 + 2 + 3 + 4 + 5 + 6 + 7)
", true);
        }
    }
}