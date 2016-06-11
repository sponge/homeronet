using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Client
{
    public interface IClientConfiguration
    {
        string Username { get; set; }
        string Password { get; set; }
        string ApiKey { get; set; }
    }
}
