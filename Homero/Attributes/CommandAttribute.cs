using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero
{
    [AttributeUsage(AttributeTargets.All)]
    public class CommandAttribute : Attribute
    {
        public string[] Aliases { get; set; }
        public string Help { get; set; }
    }
}