using System;

namespace Pyro.Network {
    /// <summary>
    ///     Identifies an object in a Domain
    /// </summary>
    public class NetId {
        private Guid _domain;

        public NetId(Guid domain, Id id) {
            _domain = domain;
            Id = id;
        }

        public Guid DomainId => _domain;

        public Id Id { get; }

        public override string ToString() {
            return $"NetId: @{_domain}:{Id.Value}";
        }

        public override bool Equals(object obj) {
            if (obj is null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((NetId)obj);
        }

        public bool Equals(NetId netId) {
            return netId._domain == _domain && netId.Id == Id;
        }

        public override int GetHashCode() {
            return -722353470 + _domain.GetHashCode() + Id.Value;
        }
    }
}