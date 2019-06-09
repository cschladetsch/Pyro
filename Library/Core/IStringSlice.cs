namespace Pyro
{
    /// <summary>
    /// A part of a string.
    /// </summary>
    public interface IStringSlice
    {
        string Text { get; }
        int Start { get; }
        int End { get; }
    }
}

