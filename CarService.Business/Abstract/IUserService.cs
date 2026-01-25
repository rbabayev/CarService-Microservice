using CarService.Entities.Entities;

namespace CarService.Business.Abstract
{
    public interface IUserService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UsernameIsTakenAsync(string username);
        Task AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(Guid id);
        Task UpdateUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task DeleteUserByIdAsync(string userId);
    }
}
