using FaceApi.Data;
using FaceApi.DTOs;
using FaceApi.Enums;
using FaceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceApi.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly ApiDbContext _db;
        public PresenceService(ApiDbContext db) => _db = db;

        public async Task<PresenceRecord> RegisterAsync(int userId, int schoolId, string awsFaceId, string geoLocalization, UserType userType)
        {
            var userSchool = await _db.UserSchools.FirstOrDefaultAsync(us => us.UserId == userId && us.SchoolId == schoolId);
            if (userSchool == null)
                throw new Exception("User not found in this school!");

            var record = new PresenceRecord
            {
                UserId = userId,
                SchoolId = schoolId,
                AwsFaceId = awsFaceId,
                GeoLocalization = geoLocalization,
                UserType = userType,
                DateTime = DateTime.UtcNow
            };

            _db.PresenceRecords.Add(record);
            await _db.SaveChangesAsync();
            return record;
        }

        public async Task<List<PresenceRecordDto>> FilterAsync(
     DateTime? start, DateTime? end, int? userId, int? schoolId, UserType? userType)
        {
            var query = _db.PresenceRecords
                .Include(p => p.User)
                .Include(p => p.School)
                .AsQueryable();

            if (start.HasValue)
                query = query.Where(x => x.DateTime >= start.Value);
            if (userType.HasValue)
                query = query.Where(x => x.UserType == userType);
            if (end.HasValue)
                query = query.Where(x => x.DateTime <= end.Value);
            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);
            if (schoolId.HasValue)
                query = query.Where(x => x.SchoolId == schoolId);

            var uaeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");

            var result = await query
                .OrderByDescending(x => x.DateTime)
                .Select(x => new PresenceRecordDto
                {
                    LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(x.DateTime, uaeTimeZone),
                    UserName = x.User.Name,
                    SchoolName = x.School.Name,
                    UserType = x.User.UserType
                })
                .ToListAsync();

            return result;
        }
    }
}
