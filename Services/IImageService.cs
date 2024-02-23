using Microsoft.AspNetCore.Http;

namespace Shop.Services
{
    public interface IImageService
    {
        void UploadImageToAzureStorage(IFormFile file);
    }
}
