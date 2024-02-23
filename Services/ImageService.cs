using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO;
using System;
using Shop.Options;

namespace Shop.Services
{
    public class ImageService : IImageService
    {
        private readonly AzureOptions _azureOptions;
        public ImageService(IOptions<AzureOptions> azureOptions) {
            _azureOptions = azureOptions.Value;
        }
        public void UploadImageToAzureStorage(IFormFile file)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        }
    }
}
