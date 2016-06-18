using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Client
{
    public class AuthenticationConfigurationRoot
    {
        public Dictionary<string, ClientAuthenticationConfiguration> Clients { get; set; } 
    }
}
