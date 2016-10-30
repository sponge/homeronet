using System.Collections.Generic;

namespace Homero.Core
{
    public interface IChannel : ISendable
    {
        string Name { get; }
        string Topic { get; }
        List<IUser> Users { get; }
    }
}