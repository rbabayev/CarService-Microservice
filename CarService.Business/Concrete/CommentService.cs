using CarService.Business.Abstract;
using CarService.DataAccess.Abstract;
using CarService.Entities.Entities;

namespace CarService.Business.Concrete
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _commentRepository.GetAllAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _commentRepository.GetByIdAsync(id);
        }

        public async Task AddCommentAsync(Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Text) || comment.UserId == null)
            {
                throw new ArgumentException("Comment text and UserId cannot be null or empty.");
            }

            comment.DateTime = DateTime.Now; // Nullable DateTime'i ayarlayın
            await _commentRepository.AddAsync(comment);

        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            await _commentRepository.UpdateAsync(comment);
        }

        public async Task DeleteCommentAsync(int id)
        {
            await _commentRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByWorkerIdAsync(Guid workerId)
        {
            return await _commentRepository.GetByWorkerIdAsync(workerId);
        }

    }
}
