using System.Runtime.InteropServices;

namespace Diver
{
    public interface IFactory
    {
        IRegistry NewRegistry();
    }
}