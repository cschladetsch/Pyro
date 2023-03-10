namespace Pyro {
    using Impl;
    using System;
    using System.IO;

    public static class Io {
        public static void Register(Registry reg, ITree tree) {
            const string root = "/bin";
            var bin = tree.Resolve(Create.Pathname(root)) as IRefBase;

            bin.Scope["file_exists"] = Create.Function<string, bool>(File.Exists);
            bin.Scope["file_delete"] = Create.Function<string, bool>(File.Delete);
            bin.Scope["file_open"] = Create.Function<string, FileStream>(File.Open);
            bin.Scope["file_readtext"] = Create.Function<string, string>(File.ReadAllText);
            bin.Scope["file_writetext"] = Create.Function<string, string, bool>(File.WriteAllText);
        }

        public static class File {
            public static bool Exists(string path) {
                return System.IO.File.Exists(path);
            }

            public static bool Delete(string path) {
                System.IO.File.Delete(path);
                return true;
            }

            public static FileStream Open(string path) {
                return new FileStream(path, FileMode.OpenOrCreate);
            }

            public static string ReadAllText(string path) {
                return System.IO.File.ReadAllText(path);
            }

            public static bool WriteAllText(string path, string text) {
                if (!System.IO.File.Exists(path))
                    return false;

                try {
                    System.IO.File.WriteAllText(path, text);
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return false;
                }

                return true;
            }
        }
    }
}
