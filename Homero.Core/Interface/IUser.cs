using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Core.Interface
{
    public interface IUser : ISendable
    {
        string Name { get; }

        string Mention { get; }
    }
}