using FaceApi.Enums;
using FaceApi.Models;

namespace FaceApi.Services
{
    public interface IPresenceService
    {
        public Task<PresenceRecord> RegisterAsync(int userId, int schoolId, string awsFaceId, string geoLocalization, UserType userType);
        public Task<List<PresenceRecord>> FilterAsync(DateTime? start, DateTime? end, int? userId, int? schoolId, UserType? userType);

    }
}
