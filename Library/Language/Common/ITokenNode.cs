namespace Diver.Language
{
    public interface ITokenNode<out TEnum>
    {
        TEnum Type { get; }
    }
}