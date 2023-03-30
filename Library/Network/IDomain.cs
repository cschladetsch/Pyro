using System;
using System.Collections.Generic;
using Flow;

namespace Pyro.Network {
    /// <summary>
    ///     A collection of objects that spatially coherent across the network.
    ///     Network Domains are like local Registries, but contain network-aware objects
    ///     and have spatial awareness.
    /// </summary>
    public interface IDomain
        : IRegistry {
        /// <summary>
        ///     Make a new local Agent
        /// </summary>
        TAgent NewAgent<TAgent>(ICollection<object> dataStack) where TAgent : IAgentBase;

        /// <summary>
        ///     Make a local proxy to a remote Agent
        /// </summary>
        IFuture<TProxy> NewProxy<TProxy>(Guid domainId);
    }
}