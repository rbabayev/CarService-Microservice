using CarService.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.DataAccess.Abstract
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetAllAsync();
        Task<Chat> GetByIdAsync(int id);
        Task AddAsync(Chat chat);
        Task UpdateAsync(Chat chat);
        Task DeleteAsync(int id);
    }
}
