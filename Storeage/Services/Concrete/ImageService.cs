using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Storeage.Models;
using Storeage.Options;
using Storeage.Services.Abstract;

namespace Storeage.Services.Concrete
{
    public class ImageService : IImageService
    {
        private readonly AzureOptions _azureOptions;
        public ImageService(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }
        public void UploadImageToAzure(IFormFile file, ImageModel imageModel)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            using MemoryStream fileupload = new MemoryStream();
            file.CopyTo(fileupload);
            fileupload.Position = 0;
            BlobContainerClient blobContainerClient = new BlobContainerClient(
                _azureOptions.ConnectionString,
                _azureOptions.Container
                );
            var path = imageModel.EmpID + fileExtension;
            BlobClient blobClient = blobContainerClient.GetBlobClient(path);
            blobClient.Upload(fileupload, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/bitmap"
                }
            }, cancellationToken: default);

        }

        public async Task DeleteDocumentAsync(string blobName)
        {
            try
            {
                blobName = blobName.Replace("https://evstorage12.blob.core.windows.net/clopay/", "");
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_azureOptions.ConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_azureOptions.Container);
                var blob = cloudBlobContainer.GetBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
