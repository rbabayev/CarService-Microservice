using CarService.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.DataAccess.Abstract
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car> GetByIdAsync(int id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
    }
}
