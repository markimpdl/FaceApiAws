namespace FaceApi.Services
{
    public interface IRekognitionService
    {
        public Task CreateCollectionIfNotExistsAsync(string collectionId);
        public Task<string> IndexFaceAsync(string collectionId, string bucket, string key);
        public Task<string> SearchFaceByImageAsync(string collectionId, Stream imageStream);
    }
}
