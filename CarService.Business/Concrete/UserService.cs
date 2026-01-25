using CarService.Business.Abstract;
using CarService.DataAccess.Abstract;
using CarService.Entities.Entities;

namespace CarService.Business.Concrete
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddAsync(user);
        }

        public async Task DeleteUserByIdAsync(string userId)
        {
            await _userRepository.DeleteByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> UsernameIsTakenAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            return user != null;
        }
    }
}
