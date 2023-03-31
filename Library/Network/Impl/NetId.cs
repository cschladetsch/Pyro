using System;

namespace Pyro.Network {
    /// <summary>
    ///     Identifies an object in a Domain
    /// </summary>
    public class NetId {
        private readonly Guid _domain;

        public Id Id { get; }

        public NetId() {
            _domain = Guid.NewGuid();
            Id = new Id();
        }

        public NetId(Guid domain, Id id) {
            _domain = domain;
            Id = id;
        }

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

            return obj.GetType() == GetType() && Equals((NetId)obj);
        }

        private bool Equals(NetId netId) {
            return netId._domain == _domain && netId.Id == Id;
        }

        public override int GetHashCode() {
            return -722353470 + _domain.GetHashCode() + Id.Value;
        }
    }
}