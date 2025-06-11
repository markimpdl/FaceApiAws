using FaceApi.Data;
using FaceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceApi.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly ApiDbContext _db;
        public SchoolService(ApiDbContext db) => _db = db;

        public async Task<School> CreateAsync(string name)
        {
            var school = new School { Name = name};
            _db.Schools.Add(school);
            await _db.SaveChangesAsync();
            return school;
        }

        public async Task<List<School>> GetAllAsync() => await _db.Schools.ToListAsync();
        public async Task<School> GetByIdAsync(int id) => await _db.Schools.FindAsync(id);

    }
}
