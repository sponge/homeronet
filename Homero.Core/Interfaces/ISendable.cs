using Homero.Core.Messages.Attachments;

namespace Homero.Core
{
    public interface ISendable
    {
        void Send(string Message);

        void Send(string Message, params object[] Format);

        void Send(string Message, params IAttachment[] Attachments);
    }
}