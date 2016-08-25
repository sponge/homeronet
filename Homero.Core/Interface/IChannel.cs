using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Core.Interface
{
    public interface IChannel : ISendable
    {
        string Name { get; }
        string Topic { get; }
        List<IUser> Users { get; }
    }
}
