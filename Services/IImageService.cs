using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Shop.Services
{
    public interface IImageService
    {
        Task<string> UploadImageToAzureStorage(IFormFile file, string fileName);
        Task DeleteImage(string fileRoute);
    }
}
