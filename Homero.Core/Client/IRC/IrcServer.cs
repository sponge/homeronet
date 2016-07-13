using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Interface;
using Homero.Core.Services;
using IrcDotNet;

namespace Homero.Core.Client.IRC
{
    public class IrcServer : IServer
    {
        private StandardIrcClient _inner;
        private IUploader _uploader;
        public IrcServer(StandardIrcClient inner, IUploader uploader)
        {
            _inner = inner;
            _uploader = uploader;
        }

        public string Name
        {
            get { return _inner.ServerName; }
        }

        public List<IChannel> Channels
        {
            get
            {
                return _inner.Channels.Select(x => new IrcChannel(x, _uploader)).Cast<IChannel>().ToList();
            }
        }
    }
}
