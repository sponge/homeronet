using System.Collections.Generic;

namespace Homero.Core
{
    public interface IServer
    {
        string Name { get; }
        List<IChannel> Channels { get; }
    }
}