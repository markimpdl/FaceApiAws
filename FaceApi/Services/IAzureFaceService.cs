namespace FaceApi.Services
{
    public interface IAzureFaceService
    {
        public Task CreatePersonGroupAsync(string groupId, string name);
        public Task<string> CreatePersonAsync(string groupId, string personName);
        public Task AddFaceToPersonAsync(string groupId, string personId, string photoUrl);
        public Task TrainPersonGroupAsync(string groupId);
        public Task<string> DetectFaceAsync(Stream photoStream);
        public Task<string> IdentifyAsync(string groupId, string faceId);

    }
}
