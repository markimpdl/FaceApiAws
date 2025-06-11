using FaceApi.Data;
using FaceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceApi.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly ApiDbContext _db;
        public PresenceService(ApiDbContext db) => _db = db;

        public async Task<PresenceRecord> RegisterAsync(int userId, int schoolId)
        {
            var userSchool = await _db.UserSchools.FirstOrDefaultAsync(us => us.UserId == userId && us.SchoolId == schoolId);
            if (userSchool == null)
                throw new Exception("Professor não vinculado a esta escola!");

            var record = new PresenceRecord
            {
                UserId = userId,
                SchoolId = schoolId,
                DateTime = DateTime.UtcNow
            };

            _db.PresenceRecords.Add(record);
            await _db.SaveChangesAsync();
            return record;
        }

        public async Task<List<PresenceRecord>> FilterAsync(DateTime? start, DateTime? end, int? userId, int? schoolId)
        {
            var query = _db.PresenceRecords
                .Include(p => p.User)
                .Include(p => p.School)
                .AsQueryable();

            if (start.HasValue)
                query = query.Where(x => x.DateTime >= start.Value);
            if (end.HasValue)
                query = query.Where(x => x.DateTime <= end.Value);
            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);
            if (schoolId.HasValue)
                query = query.Where(x => x.SchoolId == schoolId);

            return await query.OrderByDescending(x => x.DateTime).ToListAsync();
        }
    }
}
