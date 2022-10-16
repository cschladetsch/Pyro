namespace Pyro.Network.Unity {
    using Flow;
    using System;
    using UnityEngine;

    public class FlowBehaviour
        : MonoBehaviour
        , ITransient {
        public string Name { get; set; }
        public event TransientHandler Completed;
        public bool Active { get; }
        public IKernel Kernel { get; set; }

        public void Complete() {
            throw new NotImplementedException();
        }

        public ITransient Named(string name) {
            throw new NotImplementedException();
        }

        public ITransient AddTo(IGroup @group) {
            throw new NotImplementedException();
        }

        public ITransient Then(IGenerator next) {
            throw new NotImplementedException();
        }

        public ITransient Then(Action action) {
            throw new NotImplementedException();
        }

        public ITransient Then(Action<ITransient> action) {
            throw new NotImplementedException();
        }
    }
}