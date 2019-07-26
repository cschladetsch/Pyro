namespace WinForms
{
    using System;
    using System.Reflection;

    partial class MainForm
    {
        /// <summary>
        /// Get human-readable and also practical version info.
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            var desc = asm.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            var name = asm.GetName();
            var version = name.Version;

            var built = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.MinorRevision*2);
            var b = built.ToString("yy-MM-ddTHH:mm");
            var v = $"{version.Build}.{version.Revision}";

            return $"{desc} v{v} built {b}";
        }
    }
}

