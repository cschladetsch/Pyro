namespace Pyro.Language
{
    public interface ITokenNode<out TEnum>
    {
        TEnum Type { get; }
    }
}