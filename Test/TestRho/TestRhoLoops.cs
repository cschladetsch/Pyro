using System;

namespace Pyro.Test.Rho
{
	using NUnit.Framework;

	[TestFixture]
	public class TestRhoLoops
		: TestCommon
	{
		[Test]
		public void TestAccum()
		{
			RhoRun(@"
c = 0
c = c + 1
c = c + 2
assert(c == 0 + 1 + 2)
");
		}


		// TODO [Test]
		public void TestForEachIn()
		{
			//_Exec.Verbosity = 0;
			RhoRun(@"
c = 0
for (a in {1 2})
	writeln(a)
	c = c + a
assert(c == 3)
");
		}

		// TODO [Test]
		public void TestNestedForEachIn()
		{
			RhoRun(@"
c = 0
for (a in {1 2 3})
	for (b in {4 5 6})
		'c = c + a + b

writeln(c)
assert(c == 1 + 4 + 5 + 6 + 2 + 4 + 5 + 6 + 3 + 4 + 5 + 6)
");
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
