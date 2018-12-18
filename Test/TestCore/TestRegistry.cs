using System;
using NUnit.Framework;

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
            var reg = new Impl.Registry();

            var refNum = reg.AddVal(42);
            Assert.IsNotNull(refNum);
            Assert.IsNotNull(refNum.BaseValue);

            var num = reg.Get(refNum.Id);
            Assert.AreEqual(num, 42);

            Assert.AreEqual(refNum.Get<int>(), 42);

            var refStr = reg.AddVal("Foo");
            var str = reg.Get(refStr.Id);
            Assert.AreEqual(str, "Foo");
        }

        [Test] 
        public void TestReferenceFields()
        {
            var reg = new Impl.Registry();

            IRef<Foo> foo = reg.Add(new Foo());
            //Assert.AreSame(foo.Class.T);

        }
    }
}

