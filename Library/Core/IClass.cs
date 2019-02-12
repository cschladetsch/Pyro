using System.Text;

namespace Pyro
{
    public interface IClass<T> : IClassBase
    {
        void AppendText(StringBuilder str, T obj);
    }
}
