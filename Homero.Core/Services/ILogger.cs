using System.Runtime.CompilerServices;

namespace Homero.Core.Services
{
    public interface ILogger
    {
        void Info(object message, [CallerMemberName] string callingMethod = null);

        void Warn(object message, [CallerMemberName] string callingMethod = null);
        void Debug(object message, [CallerMemberName] string callingMethod = null);
        void Error(object message, [CallerMemberName] string callingMethod = null);
    }
}