using FaceApi.Models;

namespace FaceApi.Services
{
    public interface IUserService
    {
        public Task<List<User>> GetAllAsync();
        public Task<User> GetByIdAsync(int id);
    }
}
