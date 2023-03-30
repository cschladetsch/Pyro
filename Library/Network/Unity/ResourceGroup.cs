using System.Collections.Generic;
using UnityEngine;

namespace Pyro.Network.Unity {
    /// <summary>
    ///     A group of resources that are watched by a collection of remote Clients.
    /// </summary>
    public class ResourceGroup
        : ScriptableObject {
        /// <summary>
        ///     The remote clients that are receiving updates on this object or action.
        /// </summary>
        public IList<IClient> Clients;

        public IList<Common> Resources;
    }
}