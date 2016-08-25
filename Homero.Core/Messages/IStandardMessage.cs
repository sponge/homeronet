using System.Collections.Generic;
using Homero.Core.Messages.Attachments;

namespace Homero.Core.Messages
{
    public interface IStandardMessage
    {
        string Message { get; }
        List<IAttachment> Attachments { get; }
    }
}