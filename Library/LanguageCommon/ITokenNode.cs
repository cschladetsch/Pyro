namespace Diver.LanguageCommon
{
    public interface ITokenNode<out TEnum>
    {
        TEnum Type { get; }
    }
}