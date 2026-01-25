using CarService.Entities.Entities;

namespace CarService.Business.Abstract
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int id);
        Task<IEnumerable<Comment>> GetCommentsByWorkerIdAsync(Guid workerId);
    }
}
