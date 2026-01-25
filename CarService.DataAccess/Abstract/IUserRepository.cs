using CarService.Entities.Entities;

namespace CarService.DataAccess.Abstract
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> NameIsTakenAsync(string username);
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task DeleteByIdAsync(string userId);
    }
}
