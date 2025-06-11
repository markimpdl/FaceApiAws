namespace FaceApi.Services
{
    public interface IAzureBlobService
    {
        public Task<string> UploadAsync(IFormFile file, string fileName);
    }
}
