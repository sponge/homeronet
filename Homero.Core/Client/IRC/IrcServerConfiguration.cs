using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Core.Client.IRC
{
    public class IrcServerConfiguration
    {
        public string Host { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string NickservUsername { get; set; }
        public string NickServPassword { get; set; }
        public List<string> Channels { get; set; }
    }
}
