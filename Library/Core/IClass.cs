using System.Text;

namespace Pryo
{
    public interface IClass<T> : IClassBase
    {
        void AppendText(StringBuilder str, T obj);
    }
}
