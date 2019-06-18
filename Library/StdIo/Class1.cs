using System.IO;

namespace Pyro
{
    using Impl;

    public static class Io
    {
        public static void Register(Registry reg, ITree tree)
        {
            const string root = "/bin";
            var bin = tree.Resolve(Create.Pathname(root)) as IRefBase;

            bin.Scope["file_exists"] = Create.Callable<string, bool>(File.Exists);
            bin.Scope["file_delete"] = Create.Callable<string, bool>(File.Delete);
            bin.Scope["file_open"] = Create.Callable<string, FileStream>(File.Open);
            bin.Scope["file_readtext"] = Create.Callable<string, string, bool>(File.ReadAllText);
            bin.Scope["file_writetext"] = Create.Callable<string, string, bool>(File.WriteAllText);
        }

        public static class File
        {
            public static bool Exists(string path)
            {
                return System.IO.File.Exists(path);
            }

            public static bool Delete(string path)
            {
                System.IO.File.Delete(path);
                return true;
            }

            public static System.IO.FileStream Open(string path)
            {
                return new FileStream(path, FileMode.OpenOrCreate);
            }

            public static bool ReadAllText(string path, string text)
            {
                throw new System.NotImplementedException();
            }

            public static bool WriteAllText(string path, string text)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
