using Homero.Core.Messages.Attachments;
using System.Threading.Tasks;

namespace Homero.Core.Services
{
    public interface IUploader
    {
        Task<string> Upload(ImageAttachment image);
        Task<string> Upload(AudioAttachment image);
        Task<string> Upload(FileAttachment image);
    }
}
