﻿using System;

namespace Pyro.Network
{
    /// <summary>
    /// Identifies an object in a Domain
    /// </summary>
    public class NetId
    {
        public Guid DomainId => _domain;
        public Id Id => _id;

        public override string ToString()
        {
            return $"@{_domain}:{_id}";
        }

        private Guid _domain;
        private Id _id;
    }
}

