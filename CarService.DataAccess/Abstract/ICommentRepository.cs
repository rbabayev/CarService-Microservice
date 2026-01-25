using CarService.Entities.Entities;

namespace CarService.DataAccess.Abstract
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<Comment> GetByIdAsync(int id);
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<IEnumerable<Comment>> GetByWorkerIdAsync(Guid workerId);
    }
}
