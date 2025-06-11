
using Azure.Storage.Blobs;

namespace FaceApi.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IConfiguration _config;
        public AzureBlobService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> UploadAsync(IFormFile file, string fileName)
        {
            var connectionString = _config["AzureBlob:ConnectionString"];
            var containerName = _config["AzureBlob:Container"];
            var blobContainerClient = new BlobContainerClient(connectionString, containerName);

            // Garante que o container existe
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Retorna a URL pública do arquivo no Blob
            return blobClient.Uri.ToString();
        }
    }
}
