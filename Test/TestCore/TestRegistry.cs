using NUnit.Framework;
using Pyro;
using Pyro.Impl;

namespace Diver.TestCore
{
    public class Foo
    {
        public IRef<int> Num;
    }

    [TestFixture]
    public class TestRegistry
    {
        [Test]
        public void TestValues()
        {
            var reg = new Registry();
            var num = reg.Add(42);
            Assert.IsNotNull(num);
            Assert.IsNotNull(num.Value);

            Assert.AreEqual(num.Value, 42);

            IConstRefBase cref = num;
            Assert.AreEqual(cref.Get<int>(), 42);

            var str = reg.Add("");
            str.Value = "Foo";
            Assert.AreEqual(str.Value, "Foo");
        }

        [Test]
        public void TestReferenceFields()
        {
            var reg = new Registry();
            IRef<Foo> foo = reg.Add(new Foo());
            Assert.AreSame(foo.Class.Type, typeof(Foo));
        }
    }
}

