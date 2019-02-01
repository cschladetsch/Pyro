using System.Text;

namespace Diver
{
    public interface IClass<T> : IClassBase
    {
        void AppendText(StringBuilder str, T obj);
    }
}
