using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO;
using System;
using Shop.Options;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Shop.Services
{
    public class ImageService : IImageService
    {
        private readonly BlobContainerClient _blobContainerClient;
        public ImageService(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<string> UploadImageToAzureStorage(IFormFile file, string fileName)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            await blobClient.UploadAsync(memoryStream);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task DeleteImage(string fileName)
        {
            var fileUid = ExtractUidFromUrl(fileName);
            if (fileUid != null)
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileUid);
                blobClient.Delete();
            }
        }

        static string ExtractUidFromUrl(string url)
        {
            string pattern = @"\/([^\/]+)$";
            Match match = Regex.Match(url, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}
