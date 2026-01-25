using CarService.DataAccess.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.DataAccess.Concrete.EntityFramework
{
    public class EfCommentRepository : ICommentRepository
    {
        private readonly CarServiceDbContext _context;

        public EfCommentRepository(CarServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Set<Comment>().ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);

        }

        public async Task AddAsync(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment), "Comment cannot be null.");
            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetByWorkerIdAsync(Guid workerId)
        {
            return await _context.Comments.Where(c => c.UserId == workerId).ToListAsync();
        }


    }
}
