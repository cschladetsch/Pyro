namespace Pyro.Impl
{
    /// <summary>
    /// Useful for text persistence systems, and language systems.
    ///
    /// What we really want is a std::vector_char to use as the Text source.
    /// </summary>
    public class StringSlice
        : IStringSlice
    {
        public string Text { get; }
        public int Start { get; }
        public int End { get; }
    }
}