using Storeage.Models;

namespace Storeage.Services.Abstract
{
    public interface IImageService
    {
        void UploadImageToAzure(IFormFile file,ImageModel imageModel);
        Task DeleteDocumentAsync(string blobName);
    }
}
