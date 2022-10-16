namespace Pyro.Impl {
    /// <inheritdoc />
    ///  <summary>
    ///  Useful for text persistence systems, and language systems.
    ///  What we really want is a std::vector_char to use as the Text source.
    ///  </summary>
    public class StringSlice
        : IStringSlice {
        public IString Text { get; }
        public int Start { get; }
        public int End { get; }
    }
}

