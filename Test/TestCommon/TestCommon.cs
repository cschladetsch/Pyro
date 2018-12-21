using Diver.Impl;
using NUnit.Framework;

namespace Diver.Test
{
    [TestFixture]
    public class TestCommon
    {
        [SetUp]
        public void Setup()
        {
            _reg = new Registry();
            _executor = _reg.Add(new Exec.Executor());
            _exec = _executor.Value;
        }

        protected IRegistry _reg;
        protected IRef<Diver.Exec.Executor> _executor;
        protected Diver.Exec.Executor _exec;
    }
}
