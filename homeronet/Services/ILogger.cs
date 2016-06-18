using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace homeronet.Services
{
    public interface ILogger
    {
        void Info(object message, [CallerMemberName] string callingMethod = null);

        void Warn(object message, [CallerMemberName] string callingMethod = null);
        void Debug(object message, [CallerMemberName] string callingMethod = null);
        void Error(object message, [CallerMemberName] string callingMethod = null);
    }
}
