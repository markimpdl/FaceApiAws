using FaceApi.Models;

namespace FaceApi.Services
{
    public interface ISchoolService
    {
        public Task<School> CreateAsync(string name);
        public Task<List<School>> GetAllAsync();
        public Task<School> GetByIdAsync(int id);
    }
}
