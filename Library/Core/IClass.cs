namespace Pyro
{
    using System.Text;

    /// <inheritdoc />
    /// <summary>
    /// A class for a given type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IClass<in T>
        : IClassBase
    {
        void AppendText(StringBuilder str, T obj);
    }
}

