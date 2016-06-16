using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.EventArgs;
using homeronet.Messages;

namespace homeronet.Client
{
    public interface IClient : INotifyPropertyChanged
    {
        void Initialize();
        Task<bool> Connect();
        Task SendMessage(IStandardMessage message);
        bool IsConnected { get; }
        IClientConfiguration ClientConfiguration { get; set; }
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
     }
}
