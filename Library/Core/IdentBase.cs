namespace Pyro {
    /// <summary>
    /// Common to all Identifier types.
    /// </summary>
    public class IdentBase {
        /// <summary>
        /// If Quoted the ident will not be resolved immediately by an Executor.
        /// </summary>
        public bool Quoted;

        public IdentBase(bool quoted = false)
            => Quoted = quoted;
    }
}

