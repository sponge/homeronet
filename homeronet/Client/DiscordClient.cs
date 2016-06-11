using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homeronet.EventArgs;

namespace homeronet.Client
{
    public class DiscordClient : IClient
    {
        public event EventHandler<MessageReceivedEventArgs> ConnectionStatusChanged;

        #region Constructors

        #endregion


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Async Methods
        public Task<bool> Connect()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Methods
        
        public void Initialize()
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
        #region Properties

        public bool IsConnected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IClientConfiguration ClientConfiguration
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
#endregion
    }
}
