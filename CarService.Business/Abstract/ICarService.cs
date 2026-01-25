using CarService.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarService.Business.Abstract
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
        Task DeleteCarAsync(int id);
    }
}
