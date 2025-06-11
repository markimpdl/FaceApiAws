using FaceApi.Data;
using FaceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApiDbContext _db;
        public UserService(ApiDbContext db) => _db = db;

        public async Task<List<User>> GetAllAsync()
        {
            return await _db.Users
                .Include(u => u.UserSchools).ThenInclude(us => us.School)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _db.Users
                .Include(u => u.UserSchools).ThenInclude(us => us.School)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
