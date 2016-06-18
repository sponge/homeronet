using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using homeronet.EventArgs;
using homeronet.Messages;

namespace homeronet.Client
{
    public interface IClient
    {
        string Name { get; }
        string Description { get; }
        Version Version { get; }
        void Initialize();
        Task<bool> Connect();
        Task SendMessage(IStandardMessage message);
        bool IsConnected { get; }
        IClientAuthenticationConfiguration ClientAuthenticationConfiguration { get; set; }
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
     }
}
