using System;
using System.Collections.Generic;
using Flow;
using Pyro.Impl;

namespace Pyro.Network.Impl {
    public class Domain
        : Registry
        , IDomain {
        
        public Domain() {
        }

        public TAgent NewAgent<TAgent>() where TAgent : IAgentBase {
            throw new NotImplementedException();
        }

        public TAgent NewAgent<TAgent>(ICollection<object> dataStack) where TAgent : IAgentBase {
            throw new NotImplementedException();
        }

        public IFuture<TProxy> NewProxy<TProxy>(Guid domainId) {
            throw new NotImplementedException();
        }
    }
}