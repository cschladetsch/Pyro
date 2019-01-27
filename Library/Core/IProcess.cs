namespace Pryo
{
    public interface IProcess
    {
        bool Failed { get; }
        string Error { get; }
    }
}