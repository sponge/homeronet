using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Core.Utility
{
    public static class Paths
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetCallingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string DataDirectory
        {
            get { return Path.Combine(AssemblyDirectory, "Data"); }
        }

        public static string ResourceDirectory
        {
            get
            {
                return Path.Combine(AssemblyDirectory,"Plugins","Resources");
            }
        }
    }
}
