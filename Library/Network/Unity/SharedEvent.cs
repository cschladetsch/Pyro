﻿using System.Collections.Generic;
using UnityEngine.Events;

namespace Pyro.Network.Unity
{
    using UnityEngine;
    
    /// <summary>
    /// An event that is fired locally and also across all subscribers.
    /// </summary>
    public class SharedEvent
        : Common
    {
        /// <summary>
        /// The event to watch.
        /// </summary>
        public UnityEvent Event;
    }
}