
using System.IO;
using System.Threading.Tasks;

namespace ServerLibrary.Services.Contracts
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderName);
        Task DeleteFileAsync(string fileUrl);
    }
}
