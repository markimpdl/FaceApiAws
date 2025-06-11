namespace FaceApi.Services
{
    public interface IS3StorageService
    {
        public Task<string> UploadAsync(IFormFile file, string fileName);
    }
}
