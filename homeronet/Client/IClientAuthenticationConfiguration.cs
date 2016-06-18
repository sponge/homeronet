using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Client
{
    public interface IClientAuthenticationConfiguration
    {
        string Username { get; set; }
        string Password { get; set; }
        string ApiKey { get; set; }
    }

    public class ClientAuthenticationConfiguration : IClientAuthenticationConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
}
