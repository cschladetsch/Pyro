namespace Pryo
{
    public interface IStringSlice
    {
        string Text { get; }
        int Start { get; }
        int End { get; }
        //int Length => Start - End;
    }
}