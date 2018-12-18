namespace Diver
{
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