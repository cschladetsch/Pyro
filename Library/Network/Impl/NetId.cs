namespace Pyro.Network {
    using System;

    /// <summary>
    /// Identifies an object in a Domain
    /// </summary>
    public class NetId {
        public Guid DomainId => _domain;

        public Id Id => _id;

        private Guid _domain;
        private Id _id;

        public NetId(Guid domain, Id id) {
            _domain = domain;
            _id = id;
        }

        public override string ToString()
            => $"NetId: @{_domain}:{_id.Value}";

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((NetId)obj);
        }

        public bool Equals(NetId netId)
            => netId._domain == _domain && netId._id == _id;

        public override int GetHashCode()
            => -722353470 + _domain.GetHashCode() + _id.Value;
    }
}

