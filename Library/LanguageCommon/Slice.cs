namespace Diver.LanguageCommon
{
    /// <summary>
    /// A segment in a string.
    ///
    /// May be useful to add a LineNumber field later
    /// </summary>
    public struct Slice
    {
        public int Start, End;
        public int Length => End - Start;

        public Slice(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}