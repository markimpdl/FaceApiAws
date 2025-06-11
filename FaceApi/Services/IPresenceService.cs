using FaceApi.Models;

namespace FaceApi.Services
{
    public interface IPresenceService
    {
        public Task<PresenceRecord> RegisterAsync(int userId, int schoolId);
        public Task<List<PresenceRecord>> FilterAsync(DateTime? start, DateTime? end, int? userId, int? schoolId);

    }
}
