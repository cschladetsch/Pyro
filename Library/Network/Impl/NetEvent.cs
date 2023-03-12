namespace Pyro.Network.Impl {
    // TODO: IConstRe<T>

    internal struct Null {
    };


    public class NetEvent<TReturnValue, TClass> {
        public TReturnValue Invoke(params object[] args) {
            return default;
        }
    }

    public class NetEvent<TReturnValue, TClass, T0> {
    }

    public class NetEvent<TReturnValue, Class, T0, T1> {
    }
}