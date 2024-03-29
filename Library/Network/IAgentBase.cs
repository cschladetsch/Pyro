﻿using System;

namespace Pyro.Network {
    /// <summary>
    ///     Common for all Agents.
    ///     An Agent is the server-side representation of an object
    ///     that is network addresable.
    /// </summary>
    public interface IAgentBase {
        Guid NetId { get; }
    }
}