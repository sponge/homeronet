using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Core.Interface;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Client.IRC
{
    public class IrcUser : IUser
    {
        private IrcDotNet.IrcUser _inner;
        public IrcUser(IrcDotNet.IrcUser inner)
        {
            _inner = inner;
        }
        public void Send(string Message)
        {
        }

        public void Send(string Message, params object[] Format)
        {
            throw new NotImplementedException();
        }

        public void Send(string Message, params IAttachment[] Attachments)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return _inner.NickName; }
        }

        public string Mention
        {
            get { return $"{_inner.NickName}:"; }
        }
    }
}
