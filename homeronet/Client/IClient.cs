using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.EventArgs;

namespace homeronet.Client
{
    public interface IClient : INotifyPropertyChanged
    {
        void Initialize();
        Task<bool> Connect();
        bool IsConnected { get; }
        IClientConfiguration ClientConfiguration { get; set; }
        event EventHandler<MessageReceivedEventArgs> ConnectionStatusChanged;
     }
}
