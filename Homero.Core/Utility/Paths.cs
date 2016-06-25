using System;
using System.IO;
using System.Reflection;

namespace Homero.Core.Utility
{
    public static class Paths
    {
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetCallingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string DataDirectory
        {
            get { return Path.Combine(AssemblyDirectory, "Data"); }
        }

        public static string ResourceDirectory
        {
            get { return Path.Combine(AssemblyDirectory, "Plugins", "Resources"); }
        }
    }
}