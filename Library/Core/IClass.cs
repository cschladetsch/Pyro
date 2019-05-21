using System.Text;

namespace Pyro
{
    /// <summary>
    /// A class for a given type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IClass<T> : IClassBase
    {
        void AppendText(StringBuilder str, T obj);
    }
}
