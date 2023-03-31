using System;
using System.Collections.Generic;
using Flow;
using Pyro.Impl;

namespace Pyro.Network.Impl {
    public class Domain
        : Registry
        , IDomain {
        private NetId _nextNetId;

        public Domain() {
            _nextNetId = new NetId();
        }

        public TAgent NewAgent<TAgent>(ICollection<object> dataStack) where TAgent : class, IAgentBase {
            var agent = New<TAgent>(dataStack);
            return agent as TAgent;
        }

        public IFuture<TProxy> NewProxy<TProxy>(Guid domainId) where TProxy : class, IProxyBase {
            throw new NotImplementedException();
        }

        public TAgent NewAgent<TAgent>() where TAgent : class, IAgentBase {
            throw new NotImplementedException();
        }
    }
}