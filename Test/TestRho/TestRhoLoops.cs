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
");
	    }

		[Test]
		public void TestNestedForEachIn()
		{
			RhoRun(@"
c = 0
for (a in [1,2,3,4])
	for (b in [4,5,6,7])
		c = c + b
	c = c + a
writeln(c)
");
			var str = _continuation.ToString();
			//var str = _continuation.Serialise();

	        var c = 0;
	        for (var a = 1; a < 5; ++a)
	        {
		        for (var b = 4; b < 8; ++b)
			        c += b;
		        c += a;
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